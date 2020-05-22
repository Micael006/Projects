#include <stdio.h>
#include "../data.h"
#include "function.h"
#include <malloc.h>

list * create(enum Order value)
{
  list *tmp;
  tmp = (list*)malloc(sizeof(list));
  tmp->field = value;
  tmp->next = NULL;
  tmp->prev = NULL;
  return(tmp);
}

list * add(list *tmp, enum Order value)
{
  
  list *temp, *p;
  temp = (list*)malloc(sizeof(list));
  p = tmp->next;
  tmp->next = temp;
  temp->field = value;
  temp->next = p;
  temp->prev = tmp;
  if(p != NULL)
    p->prev = temp;
  return(temp);
}

list * destroy(list *root)
{
  free(root);
  printf("%s\n", "Список удалён");
  root = NULL;
  return root;
}

list * expel(list *tmp)
{
  list *prev, *next;
  prev = tmp->prev;
  next = tmp->next;
  if(prev != NULL)
    prev->next = tmp->next;
  if(next != NULL)
    next->prev = tmp->prev;
  free(tmp);
  return(prev);
}

int listlength(list *root)
{
  int count = 0;
  list *p;
  p = root;
  if(root != NULL)
  {
    count++;
    while(p->next)
    {
      count++;
      p = p->next;
    }
  }
  printf("%s", "Длина списка = ");
  return count;
}

list * expelhead(list *root)
{
  list *temp;
  temp = root->next;
  if(temp)
    temp->prev = NULL;
  free(root);
  return(temp);
}

list * removetail(list *root, int count)
{
  int check = listlength(root);
  if((check >= count) && (count > 0))
  {
    while(root->next)
      root = root->next;
    while(count > 0)
    {
      if((check == count) && (count == 1))
        root = expelhead(root);
      else
        root = expel(root);
      count--;
    }
    if(root)
      while(root->prev)
        root = root->prev;
  }
  return(root);
}

void listprint(list *root)
{
  list *p;
  p = root;
  if(root != NULL)
  {
    do
    {
      int val;
      val = (p->field == FIRST)?0:(p->field == SECOND)?1:(p->field == THIRD)?2:(p->field == FOURTH)?3:(p->field == FIFTH)?4:(p->field == SIXTH)?5:(p->field == SEVENTH)?6:(p->field == EIGHTH)?7:(p->field == NINTH)?8:(p->field == TENTH)?9:10;
      switch(val)
      {
        case 0:
	  printf("%s ", "FIRST");
	  break;
        case 1:
	  printf("%s ", "SECOND");
	  break;
        case 2:
	  printf("%s ", "THIRD");
	  break;
        case 3:
	  printf("%s ", "FOURTH");
	  break;
        case 4:
	  printf("%s ", "FIFTH");
	  break;
        case 5:
	  printf("%s ", "SIXTH");
	  break;
        case 6:
	  printf("%s ", "SEVENTH");
	  break;
        case 7:
	  printf("%s ", "EIGHTH");
	  break;
        case 8:
	  printf("%s ", "NINETH");
	  break;
        case 9:
	  printf("%s ", "TENTH");
	  break;
        default:
          printf("%s ", "Ошибка вывода");
	  break;
      }
      p = p->next;
    } while(p != NULL);
    printf("\n");
  }
  else 
    printf("%s\n", "Список пуст");
}
