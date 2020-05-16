/*
 *  Программа создаёт динамический массив и отображает на него очередь.
 *  Лоступные операции: 
 *  1. Push - добавление элемента в очередь
 *  2. Pop - удаление элемента из очереди
 *  3. Sort - сортировка очереди
 *  4. Output - вывод очереди
 */   


#include <stdio.h>
#include <stdlib.h>

#include "data.h"

int menu(void)
{
    printf("%s\n", "1. Добавить новый элемент");
    printf("%s\n", "2. Удалить первый элемент");
    printf("%s\n", "3. Вывести очередь");
    printf("%s\n", "4. Отсортировать очередь");
    printf("%s\n", "5. Выход");
    int ans;
    scanf("%d", &ans);
    return ans;
}

int main()
{
    int *queue = NULL;
    int size_of_queue = 0;
    printf("%s\n", "Добро пожаловать!");
    int k = 0;

    while (k != 5)
    {
        k = menu();
        switch (k)
        {
        case 1:
        {
            getchar();
	    printf("%s ", "Введите число:");
	    int value;
    	    scanf("%d", &value);
            queue = push(queue, &size_of_queue, value);
	    printf("%s", "\n");
        }
        break;
        case 2:
        {
            getchar();
            queue = pop(queue,&size_of_queue);
	    printf("%s", "\n");
 
        }
        break;
        case 3:
        {
            getchar();
            output(queue,size_of_queue);
        }
        break;
        case 4:
        {
            getchar();
            sort(queue,size_of_queue);
	    printf("%s", "\n");
        }
        break;
        default:
            printf("%s\n", "Извините, в меню отсутствует такая функция, попробуйте ещё раз");
            break;
        case 5:
            break;
        }
    }

    printf("%s", "До свидания!");
    return 0;
}
