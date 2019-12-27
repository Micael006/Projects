#include<stdio.h>
#include<math.h>
const double eps = 0.000001;
double module(double a){
	return (a>=0)?a:-a;
}
double F(double x){
	double result;
	result=0.6*pow(3,x)-2.3*x-3;
	return result;
}
double iteration(double x){
	double result;
	result=x+0.6*pow(3,x)-2.3*x-3;
	return result;
}
double derivative(double x){
	double result;
	result=0.6*log(3)*pow(3,x)-2.3;
	return result;
}
void dichotomy(double a, double b, double root, double previous){
	if(F(a)*F(b)<0){
               	while(1){
               		root=(a+b)/2;
                       	if(module(F(a)-F(b))<eps){
                       	printf("%s %7.4f\n","Dichotomy result:",root);
                       	break;
               		}
                else if(F(root)*F(a)>0){a=root;}
		else if(F(root)*F(b)>0){b=root;}
                }
	}
}
void iterations(double a, double b, double root, double previous){
	while(root-previous>eps){
         	previous=root;
                root=iteration(previous);
        }
        printf("%s %7.4f\n","Iteration result:",root);
}
void Newton(double a, double b, double root, double previous){
	 while(module(root-previous)>eps){
                previous=root;
                root=previous-F(previous)/derivative(previous);
        }
        printf("%s %7.4f\n","Newton's method result:",root);
}
int main(){
	double a=2,b=3,root=(a+b)/2,previous=0;
	dichotomy(a,b,root,previous);
	iterations(a,b,root,previous);
	Newton(a,b,root,previous);
}
