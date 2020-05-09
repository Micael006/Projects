#include <stdio.h>
#include "../data.h"
#include "add_delete.h"
#include <malloc.h>
#define give_memory (cell *)malloc(sizeof(cell))
cell *leaf(cell *tmp)
{
  tmp->left = NULL;
  tmp->right = NULL;
  return tmp;
}
cell *create(t_table* table, int count_lines)//нужно дополнительно передать кол-во элементов
{
  cell *new_root = give_memory;//корень
  if(!new_root)
    printf("%s", "Out of memory");
  new_root->left = give_memory;//1-ый уровень игр
  new_root->right = give_memory;//
  new_root->left->left = give_memory;//2-ой уровень игр
  new_root->left->right = give_memory;//
  new_root->right->left = give_memory;//
  new_root->left->left->left = give_memory;//3-ий уровень игр
  new_root->left->left->right = give_memory;//
  //Начало уровня участников
  new_root->left->left->left->left = give_memory;//1-ый участник
  new_root->left->left->left->left->table.key = table[0].key;
  new_root->left->left->left->left->table.index = table[0].index;
  new_root->left->left->left->left = leaf(new_root->left->left->left->left);
  new_root->left->left->left->right = give_memory;//2-ой участник
  new_root->left->left->left->right->table.key = table[1].key;
  new_root->left->left->left->right->table.index = table[1].index;
  new_root->left->left->left->right = leaf(new_root->left->left->left->right);
  new_root->left->left->right->left = give_memory;//3-ий участник
  new_root->left->left->right->left->table.key = table[2].key;
  new_root->left->left->right->left->table.index = table[2].index;
  new_root->left->left->right->left = leaf(new_root->left->left->right->left);
  new_root->left->left->right->right = give_memory;//4-ый участник
  new_root->left->left->right->right->table.key = table[3].key;
  new_root->left->left->right->right->table.index = table[3].index;
  new_root->left->left->right->right = leaf(new_root->left->left->right->right);
  new_root->left->right->left = give_memory;//5-ый участник
  new_root->left->right->left->table.key = table[4].key;
  new_root->left->right->left->table.index = table[4].index;
  new_root->left->right->left = leaf(new_root->left->right->left);
  new_root->left->right->right = give_memory;//6-ой участник
  new_root->left->right->right->table.key = table[5].key;
  new_root->left->right->right->table.index = table[5].index;
  new_root->left->right->right = leaf(new_root->left->right->right);
  new_root->right->left->left = give_memory;//7-ой участник
  new_root->right->left->left->table.key = table[6].key;
  new_root->right->left->left->table.index = table[6].index;
  new_root->right->left->left = leaf(new_root->right->left->left);
  new_root->right->left->right = give_memory;//8-ой участник
  new_root->right->left->right->table.key = table[7].key;
  new_root->right->left->right->table.index = table[7].index;
  new_root->right->left->right = leaf(new_root->right->left->right);
  if(count_lines >= 9)
  {
    new_root->right->right = give_memory;
    if(count_lines == 9)
    {
      new_root->right->right = leaf(new_root->right->right);
      new_root->right->right->table.key = table[8].key;//9-ый участник(без пары)
      new_root->right->right->table.index = table[8].index;
    }
    if(count_lines == 10)
    {
      new_root->right->right->left = give_memory;//9-ый участник
      new_root->right->right->left->table.key = table[8].key;
      new_root->right->right->left->table.index = table[8].index;
      new_root->right->right->left = leaf(new_root->right->right->left);
      new_root->right->right->right = give_memory;//10-ый участник
      new_root->right->right->right->table.key = table[9].key;
      new_root->right->right->right->table.index = table[9].index;
      new_root->right->right->right = leaf(new_root->right->right->right);
    }
  }
  else
    new_root->right->right = NULL; 
  return new_root;
}

cell *destroy(cell *tmp)
{
  free(tmp);
  printf("%s\n", "Tree has been deleted");
  tmp = NULL;
  return tmp;
}
