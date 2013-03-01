/*
 *  Copyright (c) 2012 Sam Harwell, Tunnel Vision Laboratories LLC
 *  All rights reserved.
 *
 *  The source code of this document is proprietary work, and is not licensed for
 *  distribution. For information about licensing, contact Sam Harwell at:
 *      sam@tunnelvisionlabs.com
 */
lexer grammar GrammarHighlighterLexer;

options {
	abstract=true;
}

@header {/*
 *  Copyright (c) 2012 Sam Harwell, Tunnel Vision Laboratories LLC
 *  All rights reserved.
 *
 *  The source code of this document is proprietary work, and is not licensed for
 *  distribution. For information about licensing, contact Sam Harwell at:
 *      sam@tunnelvisionlabs.com
 */
}

@members {
protected abstract int GetMultilineCommentType(); /*{
	return _modeStack.peek()==DEFAULT_MODE ? ML_COMMENT : Action_ML_COMMENT;
}*/

protected abstract void HandleBeginArgAction();
}

tokens {
	InvalidGrammarOption,
	ValidGrammarOption,
	LCURLY,
	TOKEN_REF,
	RULE_REF,
}

LEXER       : 'lexer';
PARSER      : 'parser';
CATCH       : 'catch';
FINALLY     : 'finally';
GRAMMAR     : 'grammar';
PRIVATE     : 'private';
PROTECTED   : 'protected';
PUBLIC      : 'public';
RETURNS     : 'returns';
THROWS      : 'throws';
IMPORT      : 'import';
FRAGMENT    : 'fragment';
TOKENS      : 'tokens' IGNORED '{' -> pushMode(NonActionBrace);
OPTIONS     : 'options' IGNORED '{' -> pushMode(NonActionBrace);

// v4 only
MODE        : 'mode';
LOCALS      : 'locals';

// v3 only
TEMPLATE    : 'template';
TREE        : 'tree';
SCOPE       : 'scope';

OPEN_ELEMENT_OPTION
	:	'<'
	;

CLOSE_ELEMENT_OPTION
	:	'>'
	;

AMPERSAND : '@';

COMMA : ',';

QUESTION :	'?' ;

TREE_BEGIN : '^(' ;

LPAREN:	'(' ;

RPAREN:	')' ;

COLON :	':' ;

STAR:	'*' ;

PLUS:	'+' ;

ASSIGN : '=' ;

PLUS_ASSIGN : '+=' ;

IMPLIES : '=>' ;

REWRITE : '->' ;

SEMI:	';' ;

ROOT : '^';

BANG : '!';

OR	:	'|' ;

WILDCARD : '.' ;

ETC : '...' ;

RANGE : '..' ;

NOT :	'~' ;

LBRACK:	'['	{HandleBeginArgAction();};

RBRACK:	']'	;

LCURLY:	'{' -> type(Action_TEXT), pushMode(ActionMode);

RCURLY:	'}'	;

DOLLAR : '$' ;

LABEL
	:   IDENTIFIER IGNORED '+'? '='
	;

IDENTIFIER
	:	('a'..'z' | 'A'..'Z' | '_')
		('a'..'z' | 'A'..'Z' | '0'..'9' | '_')*
	;

INT
	:	('0'..'9')+
	;

DIRECTIVE
	:	'@' IDENTIFIER
	;

REFERENCE
	:	'$' IDENTIFIER
	;

WS
	:	(	' '
		|	'\t'
		)+
	;

NEWLINE
	:	'\r'? '\n'
	;

COMMENT
	:	'//' (~('\r' | '\n'))*
	;

ML_COMMENT
	:   '/*'                    -> pushMode(BlockComment)
	;

CHAR_LITERAL
	:	'\''
		(	'\\' .
		|	~('\r' | '\n' | '\'' | '\\')
		)*
		'\''?
	;

STRING_LITERAL
	:	'"'
		(	'\\' .
		|	~('\r' | '\n' | '"' | '\\')
		)*
		'"'?
	;

fragment
IGNORED
	:   (' ' | '\t' | '\r' | '\n')*
	;

fragment
XDIGIT
	:	'0' .. '9'
	|	'a' .. 'f'
	|	'A' .. 'F'
	;

ANYCHAR
	:   .
	;

mode BlockComment;

	BlockComment_NEWLINE : NEWLINE -> type(NEWLINE);

	CONTINUE_ML_COMMENT
		:   ~('\r' | '\n' | '*')+   {Type = GetMultilineCommentType();}
		;

	END_ML_COMMENT
		:   '*/'                    {Type = GetMultilineCommentType(); PopMode();}
		;

	ML_COMMENT_STAR
		:   '*'                     {Type = GetMultilineCommentType();}
		;

	BlockComment_ANYCHAR : .            -> type(ANYCHAR);

mode ArgAction;

	ArgAction_NEWLINE : NEWLINE         -> type(NEWLINE);

	ArgAction_LBRACK
		:   '['                         -> type(LBRACK), pushMode(ArgAction)
		;

	ArgAction_RBRACK
		:   ']'                         -> type(RBRACK), popMode
		;

	ArgAction_TEXT
		:   (   ~('[' | ']' | '{' | '}' | '/' | '\r' | '\n' | '$' | '\\' | '\'' | '"')
			)+
		;

	ArgAction_CHAR_LITERAL
		:   CHAR_LITERAL
		;

	ArgAction_STRING_LITERAL
		:   STRING_LITERAL
		;

	ArgAction_ESCAPE
		:	'\\'
			(	'n'
			|	'r'
			|	't'
			|	'b'
			|	'f'
			|	'"'
			|	'\''
			|	'\\'
			|	'>'
			|   ']'
			|	'u' XDIGIT XDIGIT XDIGIT XDIGIT
			)
		;

	ArgAction_REFERENCE
		:   REFERENCE
		;

	ArgAction_SPECIAL
		:   ('$' | '/' | '\\')          -> type(Action_TEXT)
		;

	ArgAction_ANYCHAR : .               -> type(ANYCHAR);

mode NonActionBrace;

	NonActionBrace_NEWLINE : NEWLINE    -> type(NEWLINE);
	NonActionBrace_WS : WS              -> type(WS);
	NonActionBrace_LCURLY : LCURLY      -> type(LCURLY), popMode;

mode ActionMode;

	Action_NEWLINE : NEWLINE            -> type(NEWLINE);

	Action_COMMENT
		:	'//' (~('\r' | '\n'))*
		;

	Action_ML_COMMENT
		:   '/*'                        -> pushMode(BlockComment)
		;

	Action_LCURLY
		:   '{'                         -> pushMode(ActionMode), type(Action_TEXT)
		;

	Action_RCURLY
		:   '}'                         -> popMode, type(Action_TEXT)
		;

	Action_TEXT
		:   (   ~('{' | '}' | '/' | '\r' | '\n' | '$' | '\\' | '\'' | '"')
			)+
		;

	Action_CHAR_LITERAL
		:   CHAR_LITERAL
		;

	Action_STRING_LITERAL
		:   STRING_LITERAL
		;

	Action_ESCAPE
		:	'\\'
			(	'n'
			|	'r'
			|	't'
			|	'b'
			|	'f'
			|	'"'
			|	'\''
			|	'\\'
			|	'>'
			|	'u' XDIGIT XDIGIT XDIGIT XDIGIT
			)
		;

	Action_REFERENCE
		:   REFERENCE
		;

	Action_SPECIAL
		:   ('$' | '/' | '\\')          -> type(Action_TEXT)
		;

	Action_ANYCHAR : .                  -> type(ANYCHAR);

mode LexerCharSet;

	LexerCharSet_NEWLINE : NEWLINE      -> type(NEWLINE), popMode;

	LexerCharSet_ESCAPE
		:	'\\'
			(	~('u' | '\r' | '\n')
			|	'u' XDIGIT XDIGIT XDIGIT XDIGIT
			)
		;

	LexerCharSet_INVALID_ESCAPE
		:   '\\'
		|   '\\u' (XDIGIT (XDIGIT XDIGIT?)?)?
		;

	LexerCharSet_TEXT
		:   (   ~('\\' | ']' | '\r' | '\n')
			)+
		;

	LexerCharSet_RBRACK
		:   ']'                         -> type(RBRACK), popMode
		;

