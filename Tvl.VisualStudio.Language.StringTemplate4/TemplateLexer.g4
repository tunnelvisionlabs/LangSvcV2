/*
 *  Copyright (c) 2012 Sam Harwell, Tunnel Vision Laboratories LLC
 *  All rights reserved.
 *
 *  The source code of this document is proprietary work, and is not licensed for
 *  distribution. For information about licensing, contact Sam Harwell at:
 *      sam@tunnelvisionlabs.com
 */
lexer grammar TemplateLexer;

options {
	abstract = true;
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
protected abstract bool InStringTemplateMode { get; }
}

tokens {
	OPEN_DELIMITER,
	CLOSE_DELIMITER,
	QUOTE,
	STRING,
}

LPAREN  : '(';
RPAREN  : ')';
COMMA   : ',';
DOT     : '.';
COLON   : ':';
DEFINED : '::=';
EQUALS  : '=';
AT      : '@';
LBRACK  : '[';
RBRACK  : ']';
LBRACE  : '{' ANONYMOUS_PARAMETERS? -> pushMode(AnonymousTemplate);
RBRACE  : '}';

GROUP   : 'group';
DEFAULT : 'default';
IMPORT  : 'import';
TRUE    : 'true';
FALSE   : 'false';
DELIMITERS : 'delimiters' DELIMITERS_SPEC?;

WS      : (' ' | '\t')+ -> channel(HIDDEN);
NEWLINE : ('\r' '\n'? | '\n')  -> channel(HIDDEN);

ID      : ('a'..'z' | 'A'..'Z' | '_') ('a'..'z' | 'A'..'Z' | '0'..'9' | '-' | '_')*
		;

STRING
	:   '"' -> type(QUOTE), pushMode(StringTemplate)
	;

BIGSTRING
	:   '<<' -> pushMode(BigStringTemplate)
	;

BIGSTRINGLINE
	:   '<%' -> pushMode(BigStringLineTemplate)
	;

LINE_COMMENT
	:   '//' ~('\r' | '\n')*    -> channel(HIDDEN)
	;

COMMENT
	:   '/*'                    -> channel(HIDDEN), pushMode(BlockComment)
	;

fragment
ANONYMOUS_PARAMETERS
	:   WS_CHAR* ID (WS_CHAR* COMMA WS_CHAR* ID)* WS_CHAR* '|'
	;

fragment
DELIMITERS_SPEC
	:   WS_CHAR* DELIMITER_STRING WS_CHAR* COMMA WS_CHAR* DELIMITER_STRING
	;

fragment
DELIMITER_STRING
	:   '"' (~('\r' | '\n' | '"'))+ '"'
	;

fragment
WS_CHAR
	:   ' ' | '\t' | '\r' | '\n'
	;

fragment OPEN_DELIMITER : '\uFFF0';
fragment CLOSE_DELIMITER : '\uFFF1';

ANYCHAR
	:   .
	;

mode BlockComment;

	BlockComment_NEWLINE : NEWLINE -> type(NEWLINE), channel(HIDDEN);

	CONTINUE_COMMENT
		:   ~('\r' | '\n' | '*')+   -> type(COMMENT), channel(HIDDEN)
		;

	END_COMMENT
		:   '*/'                    -> type(COMMENT), channel(HIDDEN), popMode
		;

	BLOCK_COMMENT_STAR
		:   '*'                     -> type(COMMENT), channel(HIDDEN)
		;

	BlockComment_ANYCHAR : . -> type(ANYCHAR), channel(HIDDEN);

mode TemplateComment;

	TemplateComment_NEWLINE : NEWLINE -> type(NEWLINE), channel(HIDDEN);

	TemplateComment_CONTINUE_COMMENT
		:   ~('\r' | '\n' | '!')+   -> type(COMMENT), channel(HIDDEN)
		;

	TemplateComment_END_COMMENT
		:   '!' CLOSE_DELIMITER     -> type(COMMENT), channel(HIDDEN), popMode
		;

	TemplateComment_BLOCK_COMMENT_BANG
		:   '!'                     -> type(COMMENT), channel(HIDDEN)
		;

	TemplateComment_ANYCHAR : . -> type(ANYCHAR), channel(HIDDEN);

mode AnonymousTemplate;

	AnonymousTemplate_RBRACE : RBRACE -> type(RBRACE), popMode;
	AnonymousTemplate_NEWLINE : NEWLINE -> type(NEWLINE);

	AnonymousTemplate_COMMENT
		:   OPEN_DELIMITER '!' -> type(COMMENT), pushMode(TemplateComment)
		;

	AnonymousTemplate_OPEN_DELIMITER  : OPEN_DELIMITER -> type(OPEN_DELIMITER), pushMode(TemplateExpression);
	TEXT    : (~('\\' | '}' | '\uFFF0' | '\r' | '\n'))+;
	ESCAPE_RBRACE   : '\\' (. | EOF) -> type(TEXT);

	AnonymousTemplate_ANYCHAR : . -> type(ANYCHAR);

mode AnonymousTemplateParameters;

	AnonymousTemplateParameters_WS : WS -> type(WS), channel(HIDDEN);
	AnonymousTemplateParameters_NEWLINE : NEWLINE -> type(NEWLINE), channel(HIDDEN);
	AnonymousTemplateParameters_COMMA : COMMA -> type(COMMA);

	TEMPLATE_PARAMETER : ID;
	PIPE : '|' -> popMode;

	AnonymousTemplateParameters_ANYCHAR : . -> type(ANYCHAR);

mode BigStringTemplate;

	BigStringTemplate_NEWLINE : NEWLINE -> type(NEWLINE);

	BigStringTemplate_COMMENT
		:   OPEN_DELIMITER '!' -> type(COMMENT), pushMode(TemplateComment)
		;

	BigStringTemplate_OPEN_DELIMITER : OPEN_DELIMITER -> type(OPEN_DELIMITER), pushMode(TemplateExpression);
	BigStringTemplate_TEXT
		:   (~('\\' | '>' | '\uFFF0' | '\r' | '\n'))+ -> type(TEXT)
		;

	BigStringTemplate_ESCAPE : '\\' (. | EOF) -> type(TEXT);

	BigStringTemplate_END : '>>' -> popMode;
	BigStringTemplate_LANGLE : '>' -> type(TEXT);

	BigStringTemplate_ANYCHAR : . -> type(ANYCHAR);

mode BigStringLineTemplate;

	BigStringLineTemplate_NEWLINE : NEWLINE -> type(NEWLINE);

	BigStringLineTemplate_COMMENT
		:   OPEN_DELIMITER '!' -> type(COMMENT), pushMode(TemplateComment)
		;

	BigStringLineTemplate_OPEN_DELIMITER : OPEN_DELIMITER -> type(OPEN_DELIMITER), pushMode(TemplateExpression);
	BigStringLineTemplate_TEXT
		:   (~('\\' | '%' | '\uFFF0' | '\r' | '\n'))+ -> type(TEXT)
		;

	BigStringLineTemplate_ESCAPE : '\\' (. | EOF) -> type(TEXT);

	BigStringLineTemplate_END : '%>' -> popMode;
	BigStringLineTemplate_PERCENT : '%' -> type(TEXT);

	BigStringLineTemplate_ANYCHAR : . -> type(ANYCHAR);

mode TemplateExpression;

	TemplateExpression_NEWLINE : NEWLINE -> type(NEWLINE), channel(HIDDEN);
	//TemplateExpression_AT : AT -> type(AT);
	TemplateExpression_DOT : DOT -> type(DOT);
	TemplateExpression_COMMA : COMMA -> type(COMMA);
	TemplateExpression_COLON : COLON -> type(COLON);
	TemplateExpression_LPAREN : LPAREN -> type(LPAREN);
	TemplateExpression_RPAREN : RPAREN -> type(RPAREN);
	TemplateExpression_LBRACK : LBRACK -> type(LBRACK);
	TemplateExpression_RBRACK : RBRACK -> type(RBRACK);
	TemplateExpression_EQUALS : EQUALS -> type(EQUALS);
	TemplateExpression_LBRACE : LBRACE -> type(LBRACE), pushMode(AnonymousTemplate);
	TemplateExpression_WS : WS -> type(WS), channel(HIDDEN);

	SUPER : 'super';
	IF : 'if';
	ELSEIF : 'elseif';
	ENDIF : 'endif';
	ELSE : 'else';
	END : 'end';
	ELLIPSIS : '...';
	NOT : '!';
	OR : '||';
	AND : '&&';
	SEMI : ';';
	ESCAPE : '\\' (~('"' | '\uFFF1'))*;

	REGION_END : '@end';
	REGION_ID : '@' ID;
	TemplateExpression_TRUE : TRUE -> type(TRUE);
	TemplateExpression_FALSE : FALSE -> type(FALSE);
	TemplateExpression_ID : ID -> type(ID);

	TemplateExpression_CLOSE_DELIMITER : CLOSE_DELIMITER -> type(CLOSE_DELIMITER), popMode;
	TemplateExpression_STRING
		:   '"' (~('"' | '\\') | '\\' (. | EOF))* '"' -> type(STRING)
		;

	TemplateExpression_ESCAPED_STRING
		:   '\\"' (~('"' | '\\') | ('\\' {_input.La(1) != '"'}?))* '\\"' {InStringTemplateMode}? -> type(STRING)
		;

	TemplateExpression_ANYCHAR : . -> type(ANYCHAR);

mode StringTemplate;

	StringTemplate_NEWLINE : NEWLINE -> type(NEWLINE);

	StringTemplate_COMMENT
		:   OPEN_DELIMITER '!' -> type(COMMENT), pushMode(TemplateComment)
		;

	StringTemplate_OPEN_DELIMITER : OPEN_DELIMITER -> type(OPEN_DELIMITER), pushMode(TemplateExpression);
	StringTemplate_TEXT
		:   (~('\\' | '"' | '\uFFF0' | '\r' | '\n'))+ -> type(TEXT)
		;

	StringTemplate_ESCAPE : '\\' (. | EOF) -> type(TEXT);

	StringTemplate_END : '"' -> type(QUOTE), popMode;

	StringTemplate_ANYCHAR : . -> type(ANYCHAR);

mode DelimitersOpenSpec;

	DelimitersOpenSpec_WS : WS -> type(WS);
	DelimitersOpenSpec_NEWLINE : NEWLINE -> type(NEWLINE);
	DelimitersOpenSpec_DELIMITER_STRING : DELIMITER_STRING;
	DelimitersOpenSpec_COMMA : COMMA -> type(COMMA), mode(DelimitersCloseSpec);

mode DelimitersCloseSpec;

	DelimitersCloseSpec_WS : WS -> type(WS);
	DelimitersCloseSpec_NEWLINE : NEWLINE -> type(NEWLINE);
	DelimitersCloseSpec_DELIMITER_STRING : DELIMITER_STRING -> popMode;
