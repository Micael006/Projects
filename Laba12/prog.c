#include<stdio.h>

int main()
{
	long long a,c,d;
	int ch,k,length,lc;
	while(scanf("%lld",&a)!=EOF)
	{
	ch=0;
	length=0;
	lc=0;
	d=0;
	while(a%10==0)
	{
		ch++;
		a=a/10;
	}
	c=a;
	while(c!=0)
	{
		length++;
		c=c/10;
	}
	for(int i=0;i<length/2;i++)
	{
		c=c*10 + a%10;
		a=a/10;
		lc++;
	}
	while(c!=0)
	{
		d=d*10+c%10;
		c=c/10;
	}
	for(int j=0;j<ch+lc;j++)
	{
		a=a*10;
	}
	printf("%lld\n",a+d);
	}
}
