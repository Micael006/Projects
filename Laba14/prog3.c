#include<stdio.h>

struct matrix{
	int a[8][8];
	int n;
};
int get_matrix(struct matrix* m){
	if(scanf("%d", &m->n) == EOF)
		return 0;	
	for(int i=0;i<m->n;i++)
	{
		for(int j=0;j<m->n;j++)
		{
			scanf("%d", &m->a[i][j]);
		}
        }
	return 1;
}
int main()
{
	enum states{before_middle,after_middle} state;
	struct matrix m;
	int i,j;
	while(get_matrix(&m))
	{
		state=before_middle;
		i=0;
		j=0;
		printf("%d ",m.a[i][j]);
		j++;
		while((i+1)*(j+1)<m.n*m.n)
		{	
			while((j>0)&&(i<m.n-1))
        		{
                		printf("%d ",m.a[i][j]);
                		i++;
                		j--;
        		}
        		printf("%d ",m.a[i][j]);
			if((state!=after_middle)&&(((i==m.n-1)&&(j==0))||((i==0)&&(j==m.n-1)))){state=after_middle;}
			(state==after_middle)?j++:i++;
			if((i+1)*(j+1)==m.n*m.n){break;}
			while((i>0)&&(j<m.n-1))
        		{
                		printf("%d ", m.a[i][j]);
                		j++;
                		i--;
        		}
        		printf("%d ", m.a[i][j]);
			if((state!=after_middle)&&(((i==m.n-1)&&(j==0))||((i==0)&&(j==m.n-1)))){state=after_middle;}
			(state==after_middle)?i++:j++;
		}
		(m.n>1)?printf("%d \n",m.a[m.n-1][m.n-1]):printf("\n");
	}
}
