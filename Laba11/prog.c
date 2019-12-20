#include<stdio.h>
int mod(int a, int b);
void caesar(char a, int key){
        int code;
        code =(int)a;
        if(code<91){
        code =(int)a-65-key;
        (code<0)?printf("%c",(char)(123+code)):printf("%c",(char)(code+65));
        }
        else{
        code = mod(((int)a-96-key),26);
        printf("%c",(char)(code+96));
        }
}
int mod(int a, int b){
        return (a%b>0)?a%b:(a%b+b);
}
int main(){
	enum states {letter,not_a_letter} state;
	char c,key=1,k=0;
	while(scanf("%c",&c)!=EOF){
	if(((c<'a')||(c>'z'))&&((c<'A')||(c>'Z'))){
		state=not_a_letter;
	}
	else{state=letter;}
	switch(state){
		case letter:
			if(k!=0){
				k=0;
				key++;
			}
			caesar(c,key);
			break;
		case not_a_letter:
			if(c=='\n'){
				printf("%c",c);
				key=1;
			}
			else{
				printf("%c",c);
				k++;
			}	
	}
	}
}
