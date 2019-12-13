#include<stdio.h>
int mod(int a, int b);
void caesar(char a, int key)
{
	int code;
	code =(int)a;
	if(code<91)
	{
	code =(int)a-65-key;
	(code<0)?printf("%c",(char)(123+code)):printf("%c",(char)(code+65));
	}
	else
	{
	code = mod(((int)a-96-key),26);
	printf("%c",(char)(code+96));
	}
}
int mod(int a, int b)
{
	return (a%b>0)?a%b:(a%b+b);
}
int main()
{
	char h;
	int key=1;
	while(scanf("%c",&h)!=EOF)
	{
		if(h==' ')
		{
			key++;
			printf("%c",h);
		}
		else if(h=='\n')
		{
			key=1;
			printf("%c",h);
		}
		//printf("%c",(char)key)
		else
		{
			caesar(h,key);
		}
	}
}
