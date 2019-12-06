#include<stdio.h>
int fact(int in)
{
	return (in*fact(in-1));
}
int main(){
	int num;
	while(scanf("%d",&num) != EOF)
		{
			printf("%d\n",fact(num));
		}
	printf("\n");
	return 0;
}
