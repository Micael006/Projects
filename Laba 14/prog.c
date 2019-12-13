#include<stdio.h>

int main()
{
	int A[7][7],N,i=0,j=0,k,l;
	while(scanf("%d",&A[i][j])!=EOF)
	{
	k=0;
	l=0;
	i=0;
	j=0;
	N=A[i][j];
	for(i=0;i<N;i++)
	{
		for(j=0;j<N;j++)
		{
			scanf("%d",&A[i][j]);
		}
	}
	printf("%d ",A[0][0]);
	i=0;
	j=0;
	while((i!=N-1)||(j!=N-1))
	{
		if((i==0)&&(j<N-1)&&(k==0))
		{
			j++;
			printf("%d ",A[i][j]);
			k=1;
		}
		else if((k==1)&&(i+j<N-1))
                {
                        if(j==0)
                        {
                                k=0;
                        }
			else
			{
				j--;
                        	i++;
                        	printf("%d ",A[i][j]);
			}
                }
                else if((l==1)&&(i+j==N-1))
                {
                        if(i==0)
                        {
                                l=0;
                        }
			else
			{
				j++;
                        	i--;
                        	printf("%d ",A[i][j]);
			}
                }
                else if((k==1)&&(i+j==N-1))
                {
                        if(j==0)
                        {
                                k=0;
                        }
			else
			{
				j--;
                        	i++;
                       		printf("%d ",A[i][j]);
			}
                }
		else if((l==1)&&(i+j>N-1))
                {
                        if(i==N-1)
                        {
                                l=0;
                        }
			else
			{
	                        j--;
                        	i++;
                        	printf("%d ",A[i][j]);
			}
                }
                else if((k==1)&&(i+j>N-1))
                {
                        if(j==N-1)
                        {
                                k=0;
                        }
			else
			{
				j++;
                       		i--;
 	               		printf("%d ",A[i][j]);
			}
                }
                else if((l==1)&&(i+j<N-1))
                {
                        if(i==0)
                        {
                                l=0;
                        }
			else
			{
				j++;
                        	i--;
                        	printf("%d ",A[i][j]);
			}
                }
		else if((j==0)&&(i<N-1)&&(l==0))
		{
			i++;
			printf("%d ",A[i][j]);
			l=1;
		}
		else if((i==N-1)&&(j<N-1)&&(k==0))
		{
			j++;
			printf("%d ",A[i][j]);
			k=1;
		}
		else if((j==N-1)&&(i<N-1)&&(l==0))
		{
			i++; 
			printf("%d ",A[i][j]);
			l=1;
		}	
	}
	printf("\n");
	}
}
