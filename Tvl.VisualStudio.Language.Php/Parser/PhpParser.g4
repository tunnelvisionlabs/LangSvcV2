parser grammar PhpParser;

options {
	tokenVocab=PhpLexer;
}

@members
{
	protected const int EOF = Eof;
}

compileUnit
@version{4}
	:	(htmlText? code)*
		htmlText?
		EOF
	;

htmlText
	:	~HTML_START_CODE+
	;

code
@version{3}
	:	HTML_START_CODE codeElement* (CLOSE_PHP_TAG | EOF)
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
@version{2}
	:	'function' AND? PHP_IDENTIFIER functionParameterList codeBlock
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
@version{1}
	:	EQ
		(	~(COMMA|LPAREN|RPAREN)
		|	nestedParens
		|	stringLiteral
		)*
	;

nestedParens
	:	LPAREN
			(	~(LPAREN|RPAREN)
			|	nestedParens
			)*
		RPAREN
	;

stringLiteral
@version{1}
	:	PHP_SINGLE_STRING_LITERAL
	|	doubleStringLiteral
	|	hereDocStringLiteral
	;

doubleStringLiteral
@version{1}
	:	START_DOUBLE_STRING
			~END_DOUBLE_STRING*
		END_DOUBLE_STRING
	;

hereDocStringLiteral
@version{1}
	:	PHP_HEREDOC_START
			PHP_HEREDOC_TEXT*
		PHP_HEREDOC_END
	;
