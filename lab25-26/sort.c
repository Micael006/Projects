#include "data.h"

void sort(int * queue,int size_of_queue)
{
  int tmp = 0;
  for (int j = 0; j < size_of_queue; j++)
  {
    for (int i = 1; i < size_of_queue; i++)
    { 
      if (queue[i] < queue[i - 1])
      {
        int count = 0;
        while(count < size_of_queue)
        {
          tmp = queue[0];
          if(count == i-1)
          {
	    int tmp2 = queue[1];
	    queue = pop(queue, &size_of_queue);
	    queue = push(queue, &size_of_queue, tmp2);
	    queue = pop(queue, &size_of_queue);
	    queue = push(queue, &size_of_queue, tmp);
	    count += 2;
          }
	  else
	  {
            queue = pop(queue, &size_of_queue);
            queue = push(queue, &size_of_queue, tmp);
	    count++;
	  }
        }
      }
    }
  }
}
