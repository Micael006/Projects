#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "data.h"
#include "add_delete/add_delete.h"
typedef struct 
{
    char verse[100];
} t_input;
void destroy_table(t_table *table)
{
    for (int i = 0; i < 10; i++)
    {
        table[i].key=0;
        table[i].index=0;
    }
    
}
void destroy_input(t_input *input)
{
    for (int i = 0; i < 10; i++)
        input[i].verse[0]=0;
}
void enter(FILE * f, t_table *table, t_input *input, int *count_lines)
{
    fseek(f, 0L, SEEK_SET);
    while (fscanf(f, "%f %s", &table[*count_lines].key, input[*count_lines].verse) != EOF)
    {
        table[*count_lines].index = *count_lines;
        ++(*count_lines);
    }
}
t_table compare_keys(t_table table1, t_table table2)
{
  if((table1.key <= table2.key))
    return table1;
  else
    return table2;
}
t_table compare_cells(cell *left, cell *right)
{
  t_table new_table;
  if((left->table.index < 10) && (right->table.index < 10))
    new_table = compare_keys(left->table, right->table);
  else if(left->table.index < 10)
  {  
    new_table.key = left->table.key;
    new_table.index = left->table.index;
  }
  else if(right->table.index < 10)
  {
    new_table.key = right->table.key;
    new_table.index = right->table.index;
  }
  else
  {
    new_table.key = left->table.key;
    new_table.index = left->table.index;
  }
  return new_table;
}
int tournament_sort(t_table *table, int count_lines)
{
  cell *tmp = create(table, count_lines);
  for(int i = 0; i < count_lines; i++)
  {
    tmp->left->left->left->table = compare_cells(tmp->left->left->left->left, tmp->left->left->left->right);//игры 3-его уровня
    tmp->left->left->right->table = compare_cells(tmp->left->left->right->left, tmp->left->left->right->right);//
    tmp->left->left->table = compare_cells(tmp->left->left->left, tmp->left->left->right);//игры 2-ого уровня
    tmp->left->right->table = compare_cells(tmp->left->right->left, tmp->left->right->right);
    tmp->right->left->table = compare_cells(tmp->right->left->left, tmp->right->left->right);
    if(count_lines==10)
      tmp->right->right->table = compare_cells(tmp->right->right->left, tmp->right->right->right);
    tmp->left->table = compare_cells(tmp->left->left, tmp->left->right);//игры 1-ого уровня
    if(count_lines>=9)
      tmp->right->table = compare_cells(tmp->right->left, tmp->right->right);
    else
      tmp->right->table = tmp->right->left->table;
    tmp->table = compare_cells(tmp->left, tmp->right);//финал
    table[i] = tmp->table;
    table[i].index = i;
    switch (tmp->table.index)
    {
      case 0:
        tmp->left->left->left->left->table.index += 100;
	break;
      case 1:
        tmp->left->left->left->right->table.index += 100;
	break;
      case 2:
        tmp->left->left->right->left->table.index += 100;
	break;
      case 3:
        tmp->left->left->right->right->table.index += 100;
	break;
      case 4:
        tmp->left->right->left->table.index += 100;
	break;
      case 5:
        tmp->left->right->right->table.index += 100;
	break;
      case 6:
        tmp->right->left->left->table.index += 100;
	break;
      case 7:
        tmp->right->left->right->table.index += 100;
	break;
      case 8:
        if(count_lines == 9)
          tmp->right->right->table.index += 100;
        else
      	  tmp->right->right->left->table.index += 100;
	break;
      case 9:
	tmp->right->right->right->table.index += 100;
	break;
      default:
	printf("%s", "Error in sort");
	break;	
    }	    
  }
  return 1;
}
int binarysearch(float key, t_table *table, int n)
{
    int low = 0, high = n - 1;
    while (low <= high)
    {
        int middle = (low + high) / 2;
        float cmp = (key > table[middle].key) ? 1 : (key == table[middle].key) ? 0 : -1 ;
        if (cmp < 0)
            high = middle - 1;
        else if (cmp > 0)
            low = middle + 1;
        else 
            return middle;
    }
    return -1;
}
void swap_table(t_table *x, t_table *y)
{
    t_table z = *x;
    x->key = y->key;
    y->key = z.key;
}
void reverse_table(t_table *table, int count_lines)
{
    for (int i = 0; i < count_lines / 2; ++i)
        swap_table(&table[i], &table[count_lines - i - 1]);
}
void random_keys(t_table *table, int count_lines)
{
    for (int i = 0; i < count_lines; ++i)
    {
        int rand_key = rand() % count_lines;
        swap_table(&table[i], &table[rand_key]);
    }
}
void print_table(int count_lines, t_table *table, t_input *input)
{
    printf("\n|  key  | Verse\n");
    printf("----------------------------------------------------------------------\n");
    for (int i = 0; i < count_lines; i++)
        {
            printf("|  %f  |%s\n", table[i].key, input[table[i].index].verse);
            printf("----------------------------------------------------------------------\n");
        }
    printf("\n");
}
void print_menu()
{
    printf("1.Обновить таблицу.\n2.Отсортировать таблицу.\n3.Поиск по ключу.\n4.Реверс таблицы.\n5.Случайные ключи.\n6.Печать таблицы.\n7.Выход.\nВыберите действие: ");
}
void main()
{
    t_table table[10];
    t_input input[10];
    int bi, count_lines = 0, act, cs;
    float key;
    char filename[15];
    printf("Введите название файла с таблицей: ");
    scanf("%s", filename);
    FILE* f=fopen(filename, "rb");
    count_lines = 0;
    enter(f, table, input, &count_lines);
    print_menu();
    while (scanf("%d", &act)!=EOF)
    {
        printf("\n");
        switch (act)
        {
        case 1:
            fclose(f);
            count_lines = 0; cs = 0; bi = 0;
            printf("\nВведите название нового файла:");
            scanf("%s", filename);
            f=fopen(filename, "rb");
            enter(f,table,input,&count_lines);
            print_menu();
            break;
        case 2:
	    print_table(count_lines, table, input);
            cs=tournament_sort(table, count_lines);
	    print_table(count_lines, table, input);
            print_menu();
            break;
        case 3:
            printf("\n");
            printf("Введите ключ: ");
            scanf("%f", &key);
            bi=binarysearch(key, table, count_lines);
            if (cs!=1)
                printf("\nСначала отсортируйте таблицу\n");
            else if (bi==-1)
                printf("\nТакого элемента не существует!\n");
            else
                printf("|  %f  |%s\n", table[bi].key, input[table[bi].index].verse);
            print_menu();
            break;
        case 4:
            reverse_table(table, count_lines);
            print_menu();
            break;
        case 5:
            random_keys(table, count_lines);
            print_menu();
            break;
        case 6:
            print_table(count_lines, table, input);
            print_menu();
            break;
        case 7:
            exit(0);
            break;
        default:
            printf("\nПожалуйста, выберите одно из доступных действий!\n");
            print_menu();
            break;
        }
    }
    fclose(f);
}
