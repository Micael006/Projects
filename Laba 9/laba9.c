#include<stdio.h>

int min(int a,int b)
{
	return (a<b)? a:b;
}
int module(int a)
{
	return (a>=0)?a:(-a);
}
int mod(int a,int b)
{
	return ((a%b)<0)?((a%b)+b):(a%b);
}
int sign(int a)
{
	return(a == 0)? 0:(a>0)?1:(-1);
}
int check(int x, int y)
{
	return((x<=15)&&(x>=5)&&(y<=(-5))&&(y>=(-15)))? 1:0;
}
int main()
{
	const int i0=(-11),j0=(-6),l0=(-5);
	int i=i0,j=j0,k,l=l0;
	int new_i, new_j, new_l;
		for(k = 1; k<51; k++)
		{
			new_i = mod((i + j + l)*(k+1),25)-mod(i*j*l*(k+2),10) + 10;
			new_j = min(mod((i + j + l)*(k+3),25),mod(i*j*l*(k+4),25)) + 10;
			new_l = 2*sign(l)*module(mod((i+j+k)*(k+5),10) - mod(i*j*l*(k+6),25));
			if(check(new_i,new_j))
			{
				printf("%s %d %s","Success, time =",k,"\n\n");
				k = 100;
			}
			i = new_i;
			j = new_j;
			l = new_l;
		}
		if(k==51)
		{
			printf("%s %d %s %d %s %d %s %d %s", "Fail, time =", k, "\nx =", new_i, "\ny =", new_j, "\nDynamic parameter of moving =", new_l, "\n\n"); 
		}
	return 0;
}
