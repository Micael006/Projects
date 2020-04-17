
#include <stdio.h>
#include "counting_cell.h"
#include "../data.h"
//int check(cell *tmp1, cell *tmp2)//proof -> check
int check(cell *tmp)
{
    if(tmp->left && tmp->right)
    {
	return 2;
    }
    if((tmp->left && !tmp->right) || (!tmp->left && tmp->right))
    {
	return 1;
    }
    if(!tmp->left && !tmp->right)
    {
	return 0;
    }
    else
    {
	printf("%s\n", "Something went wrong");
	return (-1);
    }    
    /*if (tmp1->left && tmp2->right && !tmp1->right && !tmp2->left)
    {
        return count(tmp1->left, tmp2->right);
    }
    if (!tmp1->left && !tmp2->right && tmp1->right && tmp2->left)
    {
        return proof(tmp1->right, tmp2->left);
    }
    if (tmp1->left && tmp2->right && tmp1->right && tmp2->left)
    {
        return (proof(tmp1->right, tmp2->left) && proof(tmp1->left, tmp2->right));
    }
    if (!tmp1->left && !tmp2->right && !tmp1->right && !tmp2->left)
    {
        return 1;
    }
    else return 0;*/
}

int count(cell *tmp)
{
   int number = 0;
   if(tmp->value-48 == check(tmp))
   {
       number++;
   }
   if(tmp->right)
   {
	number+=count(tmp->right);
   }
   if(tmp->left)
   {
	number+=count(tmp->left);
   }
   return number;
}
