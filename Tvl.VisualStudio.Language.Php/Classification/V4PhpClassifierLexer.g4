lexer grammar V4PhpClassifierLexer;

@members
{
	protected const int EOF = Eof;
	protected const int HIDDEN = Hidden;
}

tokens {
	HTML_ELEMENT_NAME,
	HTML_ATTRIBUTE_NAME,
	HTML_ATTRIBUTE_VALUE,
}

WS
	:	' '
	;

NEWLINE
	:	'\r'? '\n'
	;

ANY_CHAR
	:	.
	;

mode HtmlText;

	HtmlText_NEWLINE : NEWLINE -> type(NEWLINE);

	HTML_START_CODE
		:	'<?php' -> pushMode(PhpCode)
		;

	HTML_COMMENT
		:	'<!--'
			-> pushMode(HtmlComment)
		;

	HTML_CDATA
		:	'<![CDATA['
			-> pushMode(HtmlCdata)
		;

	HTML_START_TAG
		:	{IsTagStart(input)}? '<' [/!]?
			-> pushMode(HtmlTag)
		;

	HTML_TEXT
		:	(	~[<&\r\n]
			|	{!IsTagStart(input)}? '<'
			)+
		;

	HTML_CHAR_REF
		:	'&'
			(	// named character reference
				[a-zA-Z] [a-zA-Z0-9]* ';'
			|	// decimal numeric character reference
				'#' [0-9]+ ';'
			|	// hexadecimal numeric character reference
				'#' [xX] [0-9a-fA-F]+ ';'
			)
		;

	HTML_STRAY_CHAR_REF
		:	'&' -> type(HTML_TEXT)
		;

	HtmlText_ANYCHAR : ANYCHAR -> type(ANYCHAR);

mode HtmlComment;

	HtmlComment_NEWLINE : NEWLINE -> type(NEWLINE);

	HtmlComment_TEXT : ~[\r\n'-]+ -> type(HTML_COMMENT);
	HtmlComment_DASH : '-' -> type(HTML_COMMENT);

	END_HTML_COMMENT : '-->' -> type(HTML_COMMENT), popMode;

mode HtmlCdata;

	HtmlCdata_NEWLINE : NEWLINE -> type(NEWLINE);

	HtmlCdata_DATA : ~[\r\n\]]+ -> type(HTML_CDATA);
	HtmlCdata_RBRACK : ']' -> type(HTML_CDATA);

	END_HTML_CDATA : ']]>' -> type(HTML_CDATA), popMode;

mode HtmlTag;

	HtmlTag_NEWLINE : NEWLINE -> type(NEWLINE);
	HtmlTag_WS : WS -> type(WS);

	HTML_CLOSE_TAG
		:	'/'? '>' -> popMode
		;

	HTML_OPERATOR
		:	'='
		;

	NAME
		:	~(	'\u0000'..'\u001F' | '\u007f' /*C0_CONTROL_CHAR*/
			|	'\u0080'..'\u009f' /*C1_CONTROL_CHAR*/
			|	' ' | '"' | '\'' | '>' | '/' | '='
			)+
		;

	SINGLE_QUOTE_STRING
		:	'\'' -> pushMode(SingleQuoteString)
		;

	DOUBLE_QUOTE_STRING
		:	'"' -> pushMode(DoubleQuoteString)
		;

	//fragment
	//C0_CONTROL_CHAR
	//	:	'\u0000'..'\u001F'
	//	|	'\u007f'
	//	;

	//fragment
	//C1_CONTROL_CHAR
	//	:	'\u0080'..'\u009f'
	//	;

	HtmlTag_ANYCHAR : ANYCHAR -> type(ANYCHAR);

mode SingleQuoteString;

	SingleQuoteString_NEWLINE : NEWLINE -> type(NEWLINE);

	SingleQuoteString_PHP_TAG_START : HTML_START_CODE -> type(HTML_START_CODE), pushMode(PhpCode);

	SingleQuoteString_NOT_TAG_START : '<' -> type(SINGLE_QUOTE_STRING);

	CONTINUE_SINGLE_QUOTE_STRING
		:	~(['<\r\n])+ -> type(SINGLE_QUOTE_STRING)
		;

	END_SINGLE_QUOTE_STRING
		:	'\'' -> type(SINGLE_QUOTE_STRING), popMode
		;

mode DoubleQuoteString;

	DoubleQuoteString_NEWLINE : NEWLINE -> type(NEWLINE);

	DoubleQuoteString_PHP_TAG_START : HTML_START_CODE -> type(HTML_START_CODE), pushMode(PhpCode);

	DoubleQuoteString_NOT_TAG_START : '<' -> type(DOUBLE_QUOTE_STRING);

	CONTINUE_DOUBLE_QUOTE_STRING
		:	~(["<\r\n])+ -> type(DOUBLE_QUOTE_STRING)
		;

	END_DOUBLE_QUOTE_STRING
		:	'"' -> type(DOUBLE_QUOTE_STRING), popMode
		;

mode PhpCode;

	PhpCode_NEWLINE : NEWLINE -> type(NEWLINE);
	PhpCode_WS : WS -> type(WS);

	KW___CLASS__ : '__CLASS__';
	KW___DIR__ : '__DIR__';
	KW___FILE__ : '__FILE__';
	KW___FUNCTION__ : '__FUNCTION__';
	KW___LINE__ : '__LINE__';
	KW___METHOD__ : '__METHOD__';
	KW___NAMESPACE__ : '__NAMESPACE__';
	KW_ABSTRACT : 'abstract';
	KW_AND : 'and';
	KW_AS : 'as';
	KW_BREAK : 'break';
	KW_CASE : 'case';
	KW_CATCH : 'catch';
	KW_CLASS : 'class';
	KW_CLONE : 'clone';
	KW_CONST : 'const';
	KW_CONTINUE : 'continue';
	KW_DECLARE : 'declare';
	KW_DEFAULT : 'default';
	KW_DO : 'do';
	KW_ELSE : 'else';
	KW_ELSEIF : 'elseif';
	KW_ENDDECLARE : 'enddeclare';
	KW_ENDFOR : 'endfor';
	KW_ENDFOREACH : 'endforeach';
	KW_ENDIF : 'endif';
	KW_ENDSWITCH : 'endswitch';
	KW_ENDWHILE : 'endwhile';
	KW_EXCEPTION : 'exception';
	KW_EXTENDS : 'extends';
	KW_FINAL : 'final';
	KW_FOR : 'for';
	KW_FOREACH : 'foreach';
	KW_FUNCTION : 'function';
	KW_GLOBAL : 'global';
	KW_IF : 'if';
	KW_IMPLEMENTS : 'implements';
	KW_INSTANCEOF : 'instanceof';
	KW_INTERFACE : 'interface';
	KW_NAMESPACE : 'namespace';
	KW_NEW : 'new';
	KW_OR : 'or';
	KW_PHP_USER_FILTER : 'php_user_filter';
	KW_PRIVATE : 'private';
	KW_PROTECTED : 'protected';
	KW_PUBLIC : 'public';
	KW_RETURN : 'return';
	KW_STATIC : 'static';
	KW_SWITCH : 'switch';
	KW_THIS : 'this';
	KW_THROW : 'throw';
	KW_TRY : 'try';
	KW_USE : 'use';
	KW_VAR : 'var';
	KW_WHILE : 'while';
	KW_XOR : 'xor';

	// keywords
	fragment KW_CLASS : ;
	fragment KW_EXTENDS : ;
	fragment KW_FUNCTION : ;
	fragment KW_IMPLEMENTS : ;
	fragment KW_INTERFACE : ;

	CLOSE_PHP_TAG
		:	'?>'
		;

	fragment PHP_HEREDOC_TEXT : ;
	fragment PHP_HEREDOC_END : ;

	fragment
	PHP_HEREDOC_START
		:	'<<<' PHP_IDENTIFIER {input.LA(1) == '\r' || input.LA(1) == '\n'}?
			{state.type = PHP_HEREDOC_START;}
		;

	LSHIFTEQ: '<<=';

	LSHIFT
		:	(PHP_HEREDOC_START) => PHP_HEREDOC_START {$type = state.type;}
		|	'<<'
		;

	ADDEQ	: '+=';
	SUBEQ	: '-=';
	MULEQ	: '*=';
	DIVEQ	: '/=';
	DOTEQ	: '.=';
	MODEQ	: '%=';
	ANDEQ	: '&=';
	OREQ	: '|=';
	XOREQ	: '^=';
	RSHIFTEQ: '>>=';
	ASSOC	: '=>';

	RSHIFT	: '>>';
	LE		: '<=';
	LT		: '<';
	GE		: '>=';
	GT		: '>';
	INC		: '++';
	DEC		: '--';
	TILDE	: '~';
	ARROW	: '->';
	SUB		: '-';
	AT		: '@';
	NOT		: '!';
	MUL		: '*';
	DOT		: '.';
	LTGT	: '<>';
	ANDAND	: '&&';
	AND		: '&';
	XOR		: '^';
	OROR	: '||';
	OR		: '|';
	QUESTION: '?';
	COLON	: ':';
	LBRACE	: '{';
	RBRACE	: '}';
	LBRACK	: '[';
	RBRACK	: ']';
	LPAREN	: '(';
	RPAREN	: ')';
	COMMA	: ',';
	NEQEQ	: '!==';
	NEQ		: '!=';
	EQEQEQ	: '===';
	EQEQ	: '==';
	EQ		: '=';

	fragment
	CONTINUE_HEREDOC
		:	(	options{greedy=true; k=1;} :
				PHP_IDENTIFIER (options{greedy=true; k=1;} : ';')? {CheckHeredocEnd = input.LA(1) == '\r' || input.LA(1) == '\n';}
			|	{CheckHeredocEnd = false;}
			)
			~('\r' | '\n')*
			{state.type = CONTINUE_HEREDOC;}
		;

	PHP_IDENTIFIER
		:	('a'..'z' | 'A'..'Z' | '_' | '$')
			('a'..'z' | 'A'..'Z' | '0'..'9' | '_' | '$')*
		;

	PHP_NUMBER
		:	(	'0'..'9'
			| 	'.' '0'..'9'
			)
			(PHP_IDENTIFIER PHP_NUMBER?)?
		;

	PHP_COMMENT
		:	'//' (~('\r' | '\n'))*
		;

	DOC_COMMENT_START
		:	{input.LT(4) != '/'}? => '/**'
		;

	EMPTY_BLOCK_COMMENT
		:	'/**/' {$type = PHP_ML_COMMENT;}
		;

	PHP_ML_COMMENT
		:	'/*' ~'*' .* '*/'
		;

	fragment END_SINGLE_STRING : ;
	fragment END_DOUBLE_STRING : ;

	PHP_SINGLE_STRING_LITERAL
		:	'\'' CONTINUE_SINGLE_STRING {$type = state.type;}
		;

	PHP_DOUBLE_STRING_LITERAL
		:	'"' {$type = CONTINUE_DOUBLE_STRING;}
		;

	fragment
	CONTINUE_SINGLE_STRING
		:	(	~('\r' | '\n' | '\'' | '\\')
			|	'\\' ('\'' | '\\')
			|	{input.LA(2) != '\'' && input.LA(2) != '\\'}? => '\\'
			)*
			(	'\''		{state.type = END_SINGLE_STRING;}
			|				{state.type = CONTINUE_SINGLE_STRING;}
			)
		;

	fragment DOUBLE_STRING_ESCAPE : ;

	fragment
	CONTINUE_DOUBLE_STRING
		:	'->'													{state.type = ARROW;}
		|	'['														{state.type = LBRACK;}
		|	']'														{state.type = RBRACK;}
		|	(PHP_IDENTIFIER) => PHP_IDENTIFIER						{state.type = PHP_IDENTIFIER;}
		|	(' ' | '\t')+											{state.type = WS;}
		|	{IsDoubleQuoteEscapeChar(input.LA(2))}? =>
			'\\'
			(	('n' | 'r' | 't' | 'v' | 'f' | '\\' | '$' | '"')	{state.type = DOUBLE_STRING_ESCAPE;}
			|	'0'..'7' ('0'..'7' ('0'..'7')?)?					{state.type = DOUBLE_STRING_ESCAPE;}
			|	'x'
				(	HEXDIGIT HEXDIGIT?								{state.type = DOUBLE_STRING_ESCAPE;}
				|													{state.type = CONTINUE_DOUBLE_STRING;}
				)
			)
		|	{input.LA(2) == '$'}? => '{'							{state.type = LBRACE;}
		|	{StringBraceLevel > 0}? => '}'							{state.type = RBRACE;}
		//|	PHP_NUMBER
		|	(	~('\r' | '\n' | '"' | '\\' | '$' | '-' | '[' | ']' | '{' | '}' | ' ' | '\t')
			|	{!IsDoubleQuoteEscapeChar(input.LA(2))}? => '\\'
			|	{input.LA(2) != '>'}? => '-'
			|	{input.LA(2) != '$'}? => '{'
			|	{StringBraceLevel == 0}? => '}'
			)*
			(	'"'		{state.type = END_DOUBLE_STRING;}
			|			{state.type = CONTINUE_DOUBLE_STRING;}
			)
		;

	fragment
	HEXDIGIT
		:	'0'..'9' | 'a'..'f' | 'A'..'F'
		;

	PhpCode_ANYCHAR : ANYCHAR -> type(ANYCHAR);

mode DocComment;

	DocComment_NEWLINE : NEWLINE -> type(NEWLINE);

	END_COMMENT
		:	'*/'
		;

	fragment // disables this rule for now without breaking references to the token type in code
	SVN_REFERENCE
		:	'$'
			(	~('$'|'\n'|'\r'|'*')
			|	{input.LA(2) != '/'}? => '*'
			)*
			'$'
		;

	DOC_COMMENT_TEXT
		:	//'$'? // this is a stray '$' that couldn't be made into an SVN_REFERENCE
			(	~('@' | '\\' | '\r' | '\n' | '*' /*| '$'*/)
			|	{input.LA(2) != '/'}? => '*'
			|	{!IsDocCommentStartCharacter(input.LA(2))}? => ('\\' | '@')
			)+
		;

	DOC_COMMENT_TAG
		:	('\\' | '@')
			(	('$' | '@' | '&' | '~' | '<' | '>' | '#' | '%' | '"')
			|	'\\' '::'?
			|	'f' ('$' | '[' | ']' | '{' | '}')
			|	('a'..'z' | 'A'..'Z')+
			)
		;

	fragment DOC_COMMENT_INVALID_TAG : ;

	DocComment_ANYCHAR : ANYCHAR -> type(ANYCHAR);
