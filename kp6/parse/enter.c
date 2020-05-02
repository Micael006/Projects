#include <stdio.h>
#include <stdlib.h>
#include "../data.h"
#include "../writer/writer.h"
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
  printf("%s", "Memory - ");
  scanf("%d", &form.memory);
  printf("%s", "GPU type(built_in/discrete/AGP/PCI) - ");
  scanf("%s", &tmp_type);
  form.video.typ = (tmp_type == "built_in") ? built_in : (tmp_type == "discrete") ? discrete : (tmp_type == "AGP") ? AGP : PCI;
  printf("%s", "Video card memory(GB) - ");
  scanf("%3d", &form.video.memory);
  printf("%s", "HDD type(SCSI/ATA) - ");
  scanf("%s", &tmp_type);
  form.hdd.typ = (tmp_type == "SCSI") ? SCSI_IDE : ATA_SATA;
  printf("%s", "Winchester memory(GB) - ");
  scanf("%d", &form.hdd.memory);
  printf("%s", "Number of winchester - ");
  scanf("%d", &form.hdd.count);
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
  select_type form;
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
