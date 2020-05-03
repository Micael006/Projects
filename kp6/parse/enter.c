#include <stdio.h>
#include <stdlib.h>
#include "../data.h"
#include "../writer/writer.h"
#define K 20
int compare(char* tmp)
{
  if((tmp[0] == 'b') && (tmp[1] == 'u') && (tmp[2] == 'i') && (tmp[3] == 'l') && (tmp[4] == 't') && (tmp[5] == '_') && (tmp[6] == 'i') && (tmp[7] == 'n'))
      return 0;
  else if((tmp[0] == 'd') && (tmp[1] == 'i') && (tmp[2] == 's') && (tmp[3] == 'c') && (tmp[4] == 'r') && (tmp[5] == 'e') && (tmp[6] == 't') && (tmp[7] == 'e'))
      return 1;
  else if((tmp[0] == 'A') && (tmp[1] == 'G') && (tmp[2] == 'P'))
      return 2;
  else if((tmp[0] == 'P') && (tmp[1] == 'C') && (tmp[2] == 'I'))
      return 3;
  else if((tmp[0] == 'N') && (tmp[1] == 'O') && (tmp[2] == 'N') && (tmp[3] == 'E'))
      return 4;
  else if((tmp[0] == 'S') && (tmp[1] == 'C') && (tmp[2] == 'S') && (tmp[3] == 'I'))
      return 5;
  else if((tmp[0] == 'A') && (tmp[1] == 'T') && (tmp[2] == 'A'))
      return 6;
  else return 7;
}
void fill_pc(void)
{
  str(tmp_type);
  info_pc form;
  printf("%s\n", "Please enter:");
  printf("%s", "Last name - ");
  scanf("%s", &form.last_name);
  printf("%s", "Number of processors - ");
  scanf("%d", &form.proc.count);
  printf("%s", "Type of processors - ");
  scanf("%20s", &form.proc.type);
  printf("%s", "RAM memory - ");
  scanf("%d", &form.memory);
  printf("%s", "GPU type(built_in/discrete/AGP/PCI) - ");
  scanf("%s", &tmp_type);
  if(compare(tmp_type) <= 3)
      form.video.typ = (!compare(tmp_type)) ? built_in : (compare(tmp_type) == 1) ? discrete : (compare(tmp_type) == 2) ? AGP : PCI;
  else
  {
      printf("%s\n", "ERROR! Wrong type of video was signed!");
      return;	
  }
  printf("%s", "Video card memory(GB) - ");
  scanf("%3d", &form.video.memory);
  printf("%s", "HDD type(NONE/SCSI/ATA) - ");
  scanf("%s", &tmp_type);
  if((compare(tmp_type) >= 4) && (compare(tmp_type) <= 6))
  form.hdd.typ = (compare(tmp_type) == 4) ? NONE : (compare(tmp_type) == 5) ? SCSI_IDE : ATA_SATA;//Добавил NONE
  else
  {
      printf("%s\n", "ERROR! Wrong type of HDD was signed!");
      return;
  }
  if(form.hdd.typ == NONE)//
  {			  //
      form.hdd.memory = 0;//
      form.hdd.count = 0; //
  }			  //
  else			  //
  {			  //
      printf("%s", "Winchester memory(GB) - ");
      scanf("%d", &form.hdd.memory);
      printf("%s", "Number of winchester - ");
      scanf("%d", &form.hdd.count);
  }			  //
  printf("%s", "Number of controller - ");
  scanf("%d", &form.ctrl.built_in);
  printf("%s", "Number of external devices - ");
  scanf("%d", &form.ctrl.discrete);
  printf("%s", "Operating system - ");
  scanf("%20s", &form.OC);
  form.next = NULL;
  form.last = NULL;
  
  writer(form);
}

int main()
{
  printf("%s\n", "Welcome!");
  info_pc form;
  int choice = 0;
  while (choice != 2)
  {
    printf("%s\n", "Choose your option");
    printf("%s\n", "1) Add");
    printf("%s\n", "2) Exit");
    scanf("%d", &choice);
    if (choice == 1)
    {
    fill_pc();
    }
    else if (choice != 2)
    {
      printf("%s", "Try again");
    }
    else
    {
      printf("%s\n", "Goodbye");
      return 0;
    }
  }

  return 0;
}
