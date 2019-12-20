#include<stdio.h>
long long length(long long c){
	int length=0;
	while(c!=0){
		c/=10;
		length++;
	}
	return length;
}
int main(){
	enum states{before,null,rebuild,end} state;
	long long c,half=0;
	int nul=0,pow=1,l;
	state=before;
	while(scanf("%lld",&c)!=EOF){
		switch(state){
			case before:
				if((c%10)!=0){state=end;}
				else{state=null;}
			case null:
				while((c%10==0)&&(c!=0)){
					nul++;
					c/=10;
				}
				(nul==0)?(state=end):(state=rebuild);
			case rebuild:
				l=length(c);
				for(int i=0;i<l/2;i++){
					half=half + c%10*pow;
					c/=10;
					pow*=10;
				}
				while(nul>0){
					pow*=10;
					nul--;
				}
				c=c*pow+half;
				state=end;
		 	case end:
				printf("%lld\n",c);
				state=before;
				pow=1;
				half=0;	
		}
	}
}
