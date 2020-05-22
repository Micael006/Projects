#include<stdio.h>
#include<malloc.h>
#include <stdlib.h>
#include "data.h"
#include "function/function.h"
/*
 *  Программа создаёт динамический массив и отображает на него очередь.
 *  Лоступные операции:
 *  1. Push - добавление элемента в очередь
 *  2. Pop - удаление элемента из очереди
 *  3. Sort - сортировка очереди
 *  4. Output - вывод очереди
 */
int menu(void)
{
    printf("%s\n", "1. Добавить новый элемент списка");
    printf("%s\n", "2. Удалить k-ый элемент списка");
    printf("%s\n", "3. Вывести список");
    printf("%s\n", "4. Найти длину списка");
    printf("%s\n", "5. Удалить k последних элементов списка");
    printf("%s\n", "6. Выход");
    int ans;
    scanf("%d", &ans);
    return ans;
}

int main()
{
    list *root_tmp = NULL;
    printf("%s\n", "Добро пожаловать!");
    int k = 0;

    while (k != 6)
    {
        k = menu();
        switch (k)
        {
        case 1:
          getchar();
          printf("%s\n", "Введите число от 0 до 9:");
          int value;
          scanf("%d", &value);
	  while((value > 9) || (value < 0))
          {
	    printf("%s\n", "Выход за пределы диапазона [0..9], попробуйте ещё раз");
	    scanf("%d", &value);
	  }  
	  if(!root_tmp)
	    root_tmp = create(value);
	  else
	  {
	    while(root_tmp->next)
	      root_tmp = root_tmp->next;
	    root_tmp->next = add(root_tmp, value);
	    while(root_tmp->prev)
	      root_tmp = root_tmp->prev;
	  }
          printf("%s", "\n");
        break;
        case 2:
          getchar();
	  printf("%s\n", "Введите порядковый номер элемента(начиная с единицы)");
	  int index;
	  scanf("%d", &index);
	  while(((index < 1) || (index > listlength(root_tmp))) && (listlength(root_tmp) > 0))
	  {
            printf("%s\n", "Выход за пределы списка, попробуйте ещё раз");
	    scanf("%d", &index);
	  }
	  if(listlength(root_tmp) == 0)
	    printf("%s\n", "Список пуст");
	  int current = 1;
	  while(current < index)
	  { 
	    root_tmp = root_tmp->next;
	    current++;
	  }
	  if(index == 1)
	    root_tmp = expelhead(root_tmp);
	  else 
	    root_tmp = expel(root_tmp);
	  if(root_tmp)
	    while(root_tmp->prev)
	      root_tmp = root_tmp->prev;
          printf("%s", "\n");
        break;
        case 3:
          getchar();
          listprint(root_tmp);
        break;
        case 4:
          getchar();
          printf("%d", listlength(root_tmp)); 
          printf("%s", "\n");
        break;
	case 5:
	  printf("%s\n", "Введите число удаляемых элементов");
	  int count;
	  scanf("%d", &count);
          root_tmp = removetail(root_tmp, count);
	  printf("%s", "\n");
	break;
        case 6:
            
	break;
	default:
          printf("%s\n", "Извините, в меню отсутствует такая функция, попробуйте ещё раз");
        }
    }

    printf("%s\n", "До свидания!");
    return 0;
}

