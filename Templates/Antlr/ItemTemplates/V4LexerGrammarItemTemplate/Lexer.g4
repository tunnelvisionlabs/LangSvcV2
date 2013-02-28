lexer grammar $safeitemrootname$;

@members
{
	protected const int EOF = Eof;
	protected const int HIDDEN = Hidden;
}

WS
	:	' ' -> channel(HIDDEN)
	;
