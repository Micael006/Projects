#include <stdio.h>
#include "stdlib.h"
#include "malloc.h"
#include "../data.h"
#include "reduce.h"
#include "../cleaner/cleaner.h"
#include "../builder/build.h"
cell* change(cell *tmp, char oper, int count)
{
    add_char('*', tmp);
    add_char(oper, tmp->left);
    count--;
    if(count)  
	tmp->left->right = change(tmp->left->right, oper, count);
    return tmp;
}
cell* reduce(cell *tmp)
{
   if ((tmp->left) && (tmp->type == 0) && (tmp->val.oper >= 'a') && (tmp->val.oper <= 'z')) 
   {
       if((tmp->left->type == 0) && (tmp->left->val.oper == '^') && (tmp->left->right))
       {
           if((tmp->left->right->type == 1) && (tmp->left->right->val.value > 0) && (!tmp->left->right->left) && (!tmp->left->right->right))
	   {   
               int count = tmp->left->right->val.value;
               char oper = tmp->val.oper;
               if(count == 1)
               {
	           clean(tmp->left);
	           count--; 
               }
               else if(count >= 2)
               {
	           tmp->left->val.oper = '*';
	           clean(tmp->left->right);
	           add_char(oper, tmp->left);
	           count -= 2;
	           if(count > 0)
	           {
	           tmp->left->right = change(tmp->left->right, oper, count);
	           count = 0;
	           }
               }
	       if(tmp->left->left)
       		   tmp->left->left = reduce(tmp->left->left);
   	       if(tmp->right)
       		   tmp->right = reduce(tmp->right);
           }
	   else
	   {
	   if(tmp->left)
	       tmp->left = reduce(tmp->left);
	   if(tmp->right)
	       tmp->right = reduce(tmp->right);
       	   }
       }
       else
       {
           if(tmp->left)
	       tmp->left = reduce(tmp->left);
           if(tmp->right)
	       tmp->right = reduce(tmp->right);
       }
   }
   else
   {
       if(tmp->left)
           tmp->left = reduce(tmp->left);
       if(tmp->right)
	   tmp->right = reduce(tmp->right);       
   }
   return tmp;
}
