lexer grammar PhpContentTypeLexer;

@members
{
	protected const int EOF = Eof;
	protected const int HIDDEN = Hidden;
}

NEWLINE
	:	'\r'? '\n'
	;

HTML_START_CODE
	:	('<?php' | '<?=')
		-> pushMode(PhpCode)
	;

TEXT
	:	~[<\r\n]+
	;

// handles < symbols which aren't part of HTML_START_CODE
LT
	:	'<' {true}?
		-> type(TEXT)
	;

ANYCHAR
	:	.
	;

mode PhpCode;

	PhpCode_NEWLINE : NEWLINE -> type(NEWLINE);

	CLOSE_PHP_TAG
		:	'?>' -> popMode
		;

	PHP_NOWDOC_START
		:	'<<<\'' PHP_IDENTIFIER '\'' {_input.La(1) == '\r' || _input.La(1) == '\n'}?
			-> type(TEXT), pushMode(PhpDoc)
		;

	PHP_HEREDOC_START
		:	'<<<' PHP_IDENTIFIER {_input.La(1) == '\r' || _input.La(1) == '\n'}?
			-> type(TEXT), pushMode(PhpDoc)
		;

	// have to keep this rule since it can impact PHP_HEREDOC_START and PHP_NOWDOC_START
	LSHIFT	: '<<' {true}? -> type(TEXT);

	// handles < symbols which aren't part of LSHIFT or heredoc/nowdoc start
	PhpCode_LT
		:	LT {true}? -> type(TEXT)
		;

	// handles / which is not part of // or /* to start a comment
	SLASH : '/' {true}? -> type(TEXT);

	fragment
	PHP_IDENTIFIER
		:	[a-zA-Z_$] [a-zA-Z0-9_$]*
		;

	PHP_COMMENT
		:	'//' (~('\r' | '\n'))* -> type(TEXT)
		;

	PHP_ML_COMMENT
		:	'/*' -> type(TEXT), pushMode(BlockComment)
		;

	PHP_SINGLE_STRING_LITERAL
		:	'\'' -> type(TEXT), pushMode(PhpSingleString)
		;

	PHP_DOUBLE_STRING_LITERAL
		:	'"' -> type(TEXT), pushMode(PhpDoubleString)
		;

	PhpCode_TEXT
		:	~[/'"<\r\n]+ -> type(TEXT)
		;

	PhpCode_ANYCHAR : ANYCHAR -> type(ANYCHAR);

mode BlockComment;

	BlockComment_NEWLINE : NEWLINE -> type(NEWLINE);

	BlockComment_TEXT
		:	(	~[\r\n*]
			|	'*' ~[\r\n/]
			)+
			-> type(TEXT)
		;
	BlockComment_STAR : '*' {true}? -> type(TEXT);

	END_BLOCK_COMMENT : '*/' {true}? -> type(TEXT), popMode;

mode PhpDoc;

	PhpDoc_NEWLINE : NEWLINE -> type(NEWLINE);

	PHP_DOC_END
		:	{_input.La(-1) == '\n'}?
			PHP_IDENTIFIER ';'?
			{CheckHeredocEnd(_input.La(1), Text);}?
			-> type(TEXT), popMode
		;

	PHP_DOC_TEXT
		:	~[\r\n]+ -> type(TEXT)
		;

mode PhpSingleString;

	PhpSingleString_NEWLINE : NEWLINE -> type(NEWLINE), popMode;

	PhpSingleString_ESCAPE : '\\' ('\'' | '\\')? -> type(TEXT);

	PhpSingleString_TEXT : ~['\\\r\n]+ -> type(TEXT);

	END_SINGLE_STRING : '\'' {true}? -> type(TEXT), popMode;

mode PhpDoubleString;

	PhpDoubleString_NEWLINE : NEWLINE -> type(NEWLINE);

	PhpDoubleString_ESCAPE : '\\' ('\'' | '\\')? -> type(TEXT);

	PhpDoubleString_TEXT : ~["\\\r\n]+ -> type(TEXT);

	END_DOUBLE_STRING : '"' {true}? -> type(TEXT), popMode;
