#include <stdio.h>
#include <stdlib.h>
#include "../data.h"
//надо подключить остальные хэдэры и пофиксить расположение для доступа data.h если убирать это в отдельную папку
/*int parse(cell* tmp)*/
int parse(void)//Строка для тестов
{
   char symbol; //сам символ
   int number = 0; //число
   int corruption = 0; //повреждение(да или нет)
   int pos = 0;
   while(scanf("%c",&symbol)!='\n')
   {
       int val = symbol;
       pos++;
       if((val > 47)&&(val < 58))//если встретилась цифра то строим число
       {
	   number = val-48;
           while(scanf("%c",&symbol)!=EOF)
	   {
		val = symbol;
		pos++;//увеличение позиции рассматриваемого символа
		if((val > 47)&&(val < 58))
		{
		    number = number*10 + (val - 48);
		}
		else if(symbol == ' '){break;}//пробел означает конец числа
		else//если данные повреждены, запоминаем это 
		{
		    printf("%s %d %s", "Syntax error in", pos, "position");
		    corruption = 1;
		    break;
		}
	   }
	   if(!corruption)//если данные не повреждены запоминаем число
	   {
	       //tmp = add_int(number, tmp);
	       printf("%d ", number);//Строка для тестов
	       number = 0;
	   }
       }
       else if(((val > 39)&&(val < 44))||(val == 47)||(val == 136)||((val > 96)&&(val < 123)))//если встретился символ(не минус)
       {
	   //tmp = add_char(symbol, tmp);
	   printf("%c ", symbol);//Строка для тестов
       }
       else if(val == 45)//если встретился минус
       {
	   scanf("%c",&symbol);//считываем следующий символ
	   val = symbol;       //
	   pos++;	       //
	   if(symbol == ' ')//если следующий символ - пробел, то запоминаем минус просто как символ 
	   {
	       //tmp = add_char('-', tmp);
	       printf("%c ", '-');//Строка для тестов
	   }
	   else if((val > 47)&&(val < 58))//если встретилась цифра то строим число
	   {
	       number = val-48;
               while(scanf("%c",&symbol)!='\n')
               {
                   val = symbol;
                   pos++;
                   if((val > 47)&&(val < 58))
                   {
                       number = number*10 + (val - 48);
                   }
                   else if(symbol == ' '){break;}
                   else
                   {
                       printf("%s %d %s", "Syntax error in", pos, "position");
                       corruption = 1;
		       break;
                   }
               }
               if(!corruption)
               {
                   //tmp = add_int(number, tmp);
		   printf("%d ", -number);//Строка для тестов
                   number = 0;
               }
	   }//конец построения числа(повторяющийся кусок кода)
	   else if((val > 96)&&(val < 123))//если встретилась буква, то делаем замену символа по типу -а = А
	   {
	       val = val - 32;
	       symbol = val;
	       //tmp = add_char(symbol, tmp);
	       printf("%c ", symbol);//Строка для тестов
	   }
	   else//Если в функцию попали запрещённые символы то отмечаем что данные повреждены(опть повторяющийся кусок кода)
	   {
               printf("%s %d %s", "Syntax error in", pos, "position");
               corruption = 1;
           }
       }
       else if(symbol == ' '){continue;}//Если пробел то продолжаем цикл, всё в порядке
       else if(symbol == '\n')
       {
	   pos--;
	   break;
       }
       else//Если в функцию попали запрещённые символы то отмечаем что данные повреждены
       {
	   printf("%s %d %s", "Syntax error in", pos, "position");
	   corruption = 1;
       }
       if(corruption){ return 1;}//Возвращает единицу если данные повреждены
   }
   return 0;
}
