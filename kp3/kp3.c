#include<stdio.h>
#include<math.h>
double m_eps()
{
	double e = 1.0;
	while (1.0 + e / 2.0 > 1.0){
		e /= 2.0;
	}
	return e;
}
double row(float x){
	double result = -0.2;
	for(int i = 2; i <= 100; i++){
		if((pow(2,i-1)/pow(5,i)*pow(x,i-1)) > m_eps()){
		result -= pow(2,i-1)/pow(5,i)*pow(x,i-1);
		}
		else{
		return result;
		}
	}		
}
double system_answer(float x){
	return 1/(2*x-5);
}
void print_n_times_border(int size) {
	printf("%.*s\n", size, "------------------------------------------------------------------------------");
}
void print_v(double x, double row, double system_answer, double r) {
	printf("| %6.3lf | %6.3lf | %6.3lf | %6.3lf |\n", x, row, system_answer, r);
	print_n_times_border(8 * 4 + 5);
}
int main(){
	float piece = 0.125;
	float now;
	print_n_times_border(37);
	for (int i = 0; i <= (int)(2/piece) ; i++) {
		now = piece * i;
		print_v(now, system_answer(now), row(now), system_answer(now)-row(now));
	}
	printf("Machine epsilone = %.11e\n", m_eps());
	return 0;
}
