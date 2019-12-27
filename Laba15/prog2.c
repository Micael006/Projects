#include<stdio.h>

struct matrix{
	int a[64];
	int n;
};
int get_matrix(struct matrix* m){
	if(scanf("%d",&m->n) == EOF)
		return 0;	
	for(int i=0;i<(m->n)*(m->n);i++)
	{
		scanf("%d",&m->a[i]);
        }
	return 1;
}
int sum(struct matrix* m, int current_diag){
	int k=0;
	while(((current_diag+1)%m->n!=0)&&(current_diag<m->n*m->n-m->n))
	{
	k+=m->a[current_diag];
	current_diag+=(m->n+1);
	}
	k+=m->a[current_diag];
	return k;
}
int main()
{
	struct matrix m;
	int check,n,current_i,current_diag;
	while(get_matrix(&m))
	{
		check=0;		
		current_i=2*m.n-2;
		current_diag=m.n-3;
		while(current_i<m.n*m.n-m.n)
		{
			m.a[current_i]=sum(&m,current_diag);
			current_i+=(m.n-1);
			(check==1)?(current_diag+=2*m.n):(current_diag-=2);
		       	if(current_diag==0){check=1;}
			else if(current_diag==-1){
				check=1;
				current_diag=m.n;
			}	
		}
		for(int i=0;i<m.n;i++){
			for(int j=0;j<m.n;j++){
				printf("%d ", m.a[i*m.n+j]);
			}
			printf("\n");
		}
	}
}
