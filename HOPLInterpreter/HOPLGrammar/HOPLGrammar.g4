grammar HOPLGrammar;

/*
 * Parser Rules
 */

compileUnit
	:	(importNamespace TERM)* namespaceDec+ EOF
	;

namespaceDec
	:	NAMESPACE_KW namespace CURLY_OPEN namespaceBody CURLY_CLOSE
	;

namespaceBody
	:	namespacePart*
	;

namespacePart
	:	functionDec		#functionDecNamespace
	|	globalDec TERM	#globalDecNamespace
	|	handlerDec		#handlerDecNamespace
	;

namespace
	:	ID (DOT ID)*
	;

importNamespace
	:	IMPORT_KW namespace (ALIAS_KW namespace)?
	;

identifier
	:	(namespace DOT)? ID (BOX_OPEN expr BOX_CLOSE)*
	;

args
	:	PARAN_OPEN (arg (COMMA arg)*)? PARAN_CLOSE
	;

body
	:	CURLY_OPEN stat* CURLY_CLOSE
	;

functionDec
	:	typeName ID args body
	;

handlerDec
	:	HANDLER_KW expr PARAN_OPEN (typeName ID (COMMA typeName ID)*)? PARAN_CLOSE body
	;

call
	:	identifier PARAN_OPEN (expr (COMMA expr)*)? PARAN_CLOSE
	;

stat
	:	assign TERM			#assignStat
	|	varDec TERM			#decStat
	|	return TERM			#returnStat
	|	expr TERM			#exprStat
	|	unpack TERM			#unpackStat
	|	if elseIf* else?	#ifStat
	|	while				#whileStat
	|	for					#forStat
	|	foreach				#foreachStat
	;

expr
	:	PARAN_OPEN expr PARAN_CLOSE							#paranExpr
	|	expr op=(MULT | DIV) expr							#multExpr
	|	expr op=(PLUS | MINUS) expr							#addiExpr
	|	expr op=(EQ | NEQ | LEQ | GEQ | LESS | GRT) expr	#compExpr
	|	expr AND_KW expr									#andExpr
	|	expr OR_KW expr										#orExpr
	|	expr CONCAT expr									#concatExpr
	|	MINUS expr											#negExpr
	|	NOT_KW expr											#notExpr
	|	expr BOX_OPEN expr BOX_CLOSE						#indexExpr
	|	typeVal												#valExpr
	|	call												#callExpr
	|	await												#awaitExpr
	|	identifier											#varExpr
	|	BOX_OPEN (expr (COMMA expr)*)? BOX_CLOSE			#listExpr
	|	CURLY_OPEN expr (COMMA expr)* CURLY_CLOSE			#tupleExpr
	;

await
	:	AWAIT_KW expr
	;

if
	:	IF_KW PARAN_OPEN expr PARAN_CLOSE body
	;

elseIf
	:	ELSE_KW IF_KW PARAN_OPEN expr PARAN_CLOSE body
	;

else
	:	ELSE_KW body
	;

while
	:	WHILE_KW PARAN_OPEN expr PARAN_CLOSE body
	;

for
	:	FOR_KW PARAN_OPEN declare=varDec? TERM predicate=expr TERM reeval=assign? PARAN_CLOSE body
	;

foreach
	:	FOREACH_KW PARAN_OPEN typeName ID IN_KW expr PARAN_CLOSE body
	;

arg
	:	typeName ID
	;

varDec
	:	typeName ID (ASSIGN expr)?
	;

globalDec
	:	REQUIRED_KW? CONSTANT_KW? varDec
	;

assign
	:	identifier ASSIGN expr
	;

unpack
	:	CURLY_OPEN unpacked (COMMA unpacked)* CURLY_CLOSE ASSIGN expr
	;

unpacked
	:	identifier		#idUnpacked
	|	typeName ID		#decUnpacked
	|	UIGNORE			#ignoreUnpacked
	;

return
	:	RETURN_KW expr?
	;

typeName
	:	INTEGER												#intType
	|	FLOAT												#floatType
	|	BOOLEAN												#boolType
	|	STRING												#stringType
	|	LIST_KW LESS typeName GRT							#listType
	|	TUPLE_KW LESS typeName (COMMA typeName)* GRT		#tupleType
	|	TRIGGER_KW LESS (typeName (COMMA typeName)*)? GRT	#triggerType
	|	typeName  LESS (typeName (COMMA typeName)*)? GRT	#functionType
	;

typeVal
	:	INTEGER_VAL		#intVal
	|	FLOAT_VAL		#floatVal
	|	BOOLEAN_VAL		#boolVal
	|	STRING_VAL		#stringVal
	;

/*
 * Lexer Rules
 */

// Keywords
NAMESPACE_KW	:	'namespace';
IMPORT_KW		:	'import';
ALIAS_KW		:	'as';
REQUIRED_KW		:	'required';
CONSTANT_KW		:	'const';
RETURN_KW		:	'return';
AWAIT_KW		:	'await';
TRIGGER_KW		:	'trigger';
HANDLER_KW		:	'handle';
LIST_KW			:	'list';
TUPLE_KW		:	'tuple';
IF_KW			:	'if';
ELSE_KW			:	'else';
FOR_KW			:	'for';
FOREACH_KW		:	'foreach';
IN_KW			:	'in';
WHILE_KW		:	'while';
NOT_KW			:	'not';
AND_KW			:	'and';
OR_KW			:	'or';

// Brackets
BOX_OPEN	:	'[';
BOX_CLOSE	:	']';
CURLY_OPEN	:	'{';
CURLY_CLOSE	:	'}';
PARAN_OPEN	:	'(';
PARAN_CLOSE	:	')';

// typenames
INTEGER		:	'int';
FLOAT		:	'float';
BOOLEAN		:	'bool';
STRING		:	'string';

// variable values
INTEGER_VAL	:	'-'?[0-9]+;
FLOAT_VAL	:	'-'?[0-9]+.[0-9]+;
BOOLEAN_VAL :	'true'|'false';
STRING_VAL  :	 '"' (~["\\] | '\\' (. | EOF))* '"';

// punctuation
DOT		:	'.';
COMMA	:	',';

// assignment operators
ASSIGN	:	'=';

// arithmetic operators
MULT	:	'*';
DIV		:	'/';
PLUS	:	'+';
MINUS	:	'-';

// boolean operators
EQ		:	'==';
NEQ		:	'!=';
LESS	:	'<';
LEQ		:	'<=';
GRT		:	'>';
GEQ		:	'>=';

// list operators
CONCAT	:	'::';

// unpack symbols
UIGNORE	:	'_';

// Comments
SL_COMMENT	:	'//' (~[\r\n])*	-> channel(HIDDEN);
ML_COMMENT	:	'/*' .*? '*/'	-> channel(HIDDEN);

// Default rules
ID		:	'_'?[a-zA-Z][_a-zA-Z0-9]*;
WS		:	[ \t\n\r]+ -> channel(HIDDEN);
TERM	:	';';
