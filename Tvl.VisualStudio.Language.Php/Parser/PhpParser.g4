parser grammar PhpParser;

options {
	tokenVocab=PhpLexer;
}

@members
{
	protected const int EOF = Eof;
}

compileUnit
	:	(htmlText | code)*
		EOF
	;

htmlText
	:	~HTML_START_CODE+
	;

code
	:	HTML_START_CODE codeElement* CLOSE_PHP_TAG
	;

codeElement
	:	classOrInterfaceDefinition
	|	functionDefinition
	|	codeBlock
	|	~(CLOSE_PHP_TAG|'function'|LBRACE|RBRACE|'class'|'interface')
	;

codeBlock
	:	LBRACE
			(	codeElement
			|	htmlLiteral
			)*
		RBRACE
	;

htmlLiteral
	:	CLOSE_PHP_TAG
			~HTML_START_CODE*
		HTML_START_CODE
	;

classOrInterfaceDefinition
	:	(KW_CLASS | KW_INTERFACE) PHP_IDENTIFIER extendsList? implementsList? codeBlock
	;

extendsList
	:	'extends' PHP_IDENTIFIER (COMMA PHP_IDENTIFIER)*
	;

implementsList
	:	'implements' PHP_IDENTIFIER (COMMA PHP_IDENTIFIER)*
	;

functionDefinition
	:	'function' PHP_IDENTIFIER functionParameterList codeBlock
	;

functionParameterList
	:	LPAREN functionParameters? RPAREN
	;

functionParameters
	:	functionParameter (COMMA functionParameter)*
	;

functionParameter
	:	AND? PHP_IDENTIFIER parameterDefaultValue?
	;

parameterDefaultValue
	:	EQ
		(	~(COMMA|LPAREN|RPAREN)
		|	nestedParens
		)*
	;

nestedParens
	:	LPAREN
			(	~(LPAREN|RPAREN)
			|	nestedParens
			)*
		RPAREN
	;
