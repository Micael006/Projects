#include<stdio.h>
int fact(int in)
{
	return(in != 0)? in*fact(in-1):1;
}
int main(){
	int number;
	while(scanf("%d",&number) != EOF)
		{
			(number>0)?printf("%d\n",fact(number)):printf("%s\n","error ");
		}
	printf("\n");
	return 0;
}
