#include<stdio.h>
#include<ctype.h>
#define CONSONANT (1u<<('b'-'a')|1u<<('c'-'a')|1u<<('d'-'a')|1u<<('f'-'a')|1u<<('g'-'a')|1u<<('h'-'a')|1u<<('j'-'a')|1u<<('k'-'a')|1u<<('l'-'a')|1u<<('m'-'a')|1u<<('n'-'a')|1u<<('p'-'a')|1u<<('q'-'a')|1u<<('r'-'a')|1u<<('s'-'a')|1u<<('t'-'a')|1u<<('v'-'a')|1u<<('w'-'a')|1u<<('x'-'a')|1u<<('z'-'a'))
int checking(unsigned int letters, int c)
{
	return ((1u<<(c-'a')&letters)==0)?0:1;
}
int main()
{
	int c,check1=0,check2=0,check3=0;
	while((c=getchar())!=EOF)
	{
		c=tolower(c);
		if(c<'a'||c>'z')
		{
			if(check3==0){check3=check1*check2;}
			check1=0;
			check2=0;
		}
		else
		{
			if(check1==0){check1=checking(CONSONANT,c);}
			check2=checking(CONSONANT,c);
		}
	}
	if(check3==0){check3=check1*check2;}
	(check3==1)?printf("%s\n","True"):printf("%s\n","False");
}
