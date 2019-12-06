#include<stdio.h>
int fact(int in)
{
	return(in != 0)? in*fact(in-1):1;
}
int main(){
	int num;
	while(scanf("%d",&num) != EOF)
		{
			(num>0)?printf("%d\n",fact(num)):printf("%s\n","error");
		}
	priiiiiintf("\n");
	return 0;
}
