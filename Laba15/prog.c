#include<stdio.h>

int main()
{	
	int A[8][8],N,i=0,j=0,ci,cj,sum,k;
	while(scanf("%d",&A[i][j])!=EOF)
	{
	i=0;
	j=0;
	N=A[i][j];
	k=0;
	for(i=0;i<N;i++)
	{
		for(j=0;j<N;j++)
		{
			scanf("%d",&A[i][j]);
		}
	}
	j=N-2;
	i=1;
	ci=0;
	cj=N-1;
	while((ci!=N-1)||(cj!=0))
	{
		if(cj!=0){cj-=2;}
		else if(ci!=N-1){ci+=2;}
		if(cj==-1)
		{
			cj=0;
			ci=1;
		}
		if(((cj>=0)&&(ci==0))||((ci<N-1)&&(cj==0)))
		{
			sum=0;
			k++;
			for(i=ci,j=cj;((i>=0)&&(j<=N-1))&&((j>=0)&&(i<=N-1));)
			{
				sum+=A[i][j];
				i++;
				j++;
			}
			A[k][N-1-k]=sum;
		}
	}
	for(i=0;i<N;i++)
	{
		for(j=0;j<N;j++)
		{
			printf("%d ",A[i][j]);
		}
	printf("\n");
	}
	i=0;
	j=0;
	}
}
