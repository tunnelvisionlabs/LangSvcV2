/*
 *  Copyright (c) 2012 Sam Harwell, Tunnel Vision Laboratories LLC
 *  All rights reserved.
 *
 *  The source code of this document is proprietary work, and is not licensed for
 *  distribution. For information about licensing, contact Sam Harwell at:
 *      sam@tunnelvisionlabs.com
 */
parser grammar TemplateParser;

options {
	tokenVocab=TemplateLexer;
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

groupFile
@version{4}
	:	group EOF
	;

group
	:   oldStyleHeader?
		delimiters?
		(   'import' string
		)*
		def+
	;

oldStyleHeader
@version{1}
	:   'group' ID (':' ID)? ';'
	;

groupName
	:   ID ('.' ID)*
	;

delimiters
@version{1}
	:   DELIMITERS DelimitersOpenSpec_DELIMITER_STRING ',' DelimitersCloseSpec_DELIMITER_STRING
	;

def
	:   templateDef
	|   dictDef
	;

templateDef
@version{3}
	:   (   '@' enclosing=(ID|TRUE|FALSE) '.' name=(ID|TRUE|FALSE) '(' ')'
		|   name=(ID|TRUE|FALSE) '(' formalArgs ')'
		)
		'::='
		(   stringTemplate
		|   bigstringTemplate
		|   bigstringTemplateNoNewline
		)
	|   alias=(ID|TRUE|FALSE) '::=' target=(ID|TRUE|FALSE)
	;

formalArgs
	:   formalArg (',' formalArg)*
	|
	;

formalArg
@version{1}
	:   name=ID
		(   '=' (string | anonymousTemplate | (TRUE | FALSE) | '[' ']')
		)?
	;

dictDef
	:   name=ID '::=' dict
	;

dict
	:   '[' dictPairs ']'
	;

dictPairs
	:   keyValuePair (',' keyValuePair)* (',' defaultValuePair)?
	|   defaultValuePair
	;

defaultValuePair
	:   'default' ':' keyValue
	;

keyValuePair
	:   string ':' keyValue
	;

keyValue
@version{2}
	:   bigstringTemplate
	|   bigstringTemplateNoNewline
	|   anonymousTemplate
	|   string
	|	'[' ']'
	|   (   TRUE
		|   FALSE
		|   ID
		)
	;

string
	:   stringTemplate
	;

stringTemplate
	:   QUOTE
		templateBody
		QUOTE
	;

bigstringTemplate
	:   BIGSTRING
		templateBody
		BigStringTemplate_END
	;

bigstringTemplateNoNewline
	:   BIGSTRINGLINE
		templateBody
		BigStringLineTemplate_END
	;

anonymousTemplate
	:   LBRACE
		anonymousTemplateParameters?
		templateBody
		RBRACE
	;

anonymousTemplateParameters
	:   names+=TEMPLATE_PARAMETER (COMMA names+=TEMPLATE_PARAMETER)* PIPE
	;

templateBody
	:   (   (NEWLINE | COMMENT | TEXT)
		|   ifstat
		|   region
		|   exprTag
		|   escape
		)*
	;

escape
	:   OPEN_DELIMITER ESCAPE CLOSE_DELIMITER
	;

exprTag
	:   OPEN_DELIMITER
		expr (SEMI exprOptions)?
		CLOSE_DELIMITER
	;

region
	:   OPEN_DELIMITER REGION_ID CLOSE_DELIMITER
		templateBody
		OPEN_DELIMITER REGION_END CLOSE_DELIMITER
	;

subtemplate
	:   anonymousTemplate
	;

ifstat
	:   OPEN_DELIMITER IF LPAREN conditional RPAREN CLOSE_DELIMITER
		templateBody
		(OPEN_DELIMITER ELSEIF LPAREN conditional RPAREN CLOSE_DELIMITER templateBody)*
		(OPEN_DELIMITER ELSE CLOSE_DELIMITER templateBody)?
		OPEN_DELIMITER ENDIF CLOSE_DELIMITER
	;

conditional
	:   andConditional (OR andConditional)*
	;

andConditional
	:   notConditional (AND notConditional)*
	;

notConditional
	:   NOT notConditional
	|   memberExpr
	;

exprOptions
	:   option (COMMA option)*
	;

option
	:   name=ID
		(   EQUALS value=exprNoComma
		)?
	;

exprNoComma
	:   memberExpr
		(COLON mapTemplateRef)?
	;

expr
	:   mapExpr
	;

mapExpr
	:   memberExpr
		((COMMA memberExpr)+ COLON mapTemplateRef)?
		(COLON mapTemplateRef (COMMA mapTemplateRef)*)*
	;

mapTemplateRef
	:   ID LPAREN arguments RPAREN
	|   subtemplate
	|   LPAREN mapExpr RPAREN LPAREN argExprList? RPAREN
	;

memberExpr
	:   includeExpr
		(   DOT ID
		|   DOT LPAREN mapExpr RPAREN
		)*
	;

includeExpr
	:   SUPER DOT templateName=ID LPAREN arguments RPAREN
	|   templateName=ID LPAREN arguments RPAREN
	|   AT SUPER DOT templateName=ID LPAREN RPAREN
	|   regionName=REGION_ID LPAREN RPAREN
	|   primary
	;

primary
@version{1}
	:   (   ID
		|   STRING
		|   TRUE
		|   FALSE
		)
	|   subtemplate
	|   list
	|   LPAREN expr RPAREN (LPAREN argExprList? RPAREN)?
	|   LPAREN conditional RPAREN
	;

arguments
	:   argExprList
	|   namedArg (COMMA namedArg)* (COMMA ELLIPSIS)?
	|   ELLIPSIS
	|   // empty
	;

argExprList
	:   arg (COMMA arg)*
	;

arg
	:   exprNoComma
	;

namedArg
	:   name=ID EQUALS value=arg
	;

list
	:   LBRACK RBRACK
	|   LBRACK listElement (COMMA listElement)* RBRACK
	;

listElement
	:   exprNoComma?
	;
