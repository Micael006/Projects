#!/bin/bash
# Проверить было ли передано два параметра
if [[ $# -ne 2 ]]
    then
    check="$1"
    #Если не было передано параметров вызываем начальную помощь
    if [[ $# -eq 0 ]]
       then
       echo "Please, enter some information below:"
       echo '"'"?"'"'" for help"
       echo '"'"generate"'"'" for generating new void files"
       echo "or <suffix>" 
       echo "   <username> for synonim creation"
       read check
    fi
    #Если не запрашивается генерация или помощь,
    #то считываем данные для создания синонима
    if [ $check = "?" ]||[ $check = "generate" ]
       then
	  echo " " 
       else
          # Ввод данных
          suffix="$check"
          echo "Please, enter your username below"
          read username
    fi
    while [[ "$check" = "?"||"$check" = "generate" ]]
       do 
          #Если передан знак вопроса вызываем помощь
          if [[ "$check" == "?" ]]
             then
	        echo "For help"
	        echo "Usage: ?"
                echo "For synonim creation"  
                echo "Usage: <suffix>"
                echo "       <username>"
                echo "For extra files generation"
                echo "Usage: generate"
          fi
          #Если подан запрос на генерацию файлов, 
          #генерируем, пока не будет введено что-либо кроме generate
          if [[ "$check" == "generate" ]]
             then
                echo "Please, enter the suffix of files"
                read gen_suffix
		echo "Please, enter the number of files"
                read number
		if [[ $number < 0 ]]
		   then
			echo "Negative number of files? Interesting..."
			echo "...But you know, I can't do something like that :-)"
		   else
                      count=0
                      while [ $count -lt $number ]
                         do
                            (( count++ ))
                            echo "file$count.$gen_suffix"| xargs touch
                         done
                      count=0
		fi
          fi
	     echo "Please type "'"'"?"'"'", "'"'"generate"'"'" or <suffix>"
	     echo "                               <username> for continue"
	     read fix
	     check=${fix}
	     if [[ "${check}" == "?"||"${check}" == "generate" ]]
	     	then
		       continue	
	     	else
		   suffix=${check}
		   echo "Please, enter your username below"
		   read username
		   break
	     fi
          done
   else
      check=$1
      suffix=$check
      username=$2
fi

if [[ `echo ${suffix} | cut -c 1` == "." ]]
   then
       echo "Please, remove the dot at the beginning of suffix and write down it below again:"
       read suffix
fi
 
if [ ! -d "./synonyms" ]; then
    # Создать папку для ссылок (синонимов), только если ее не было
    mkdir synonyms
fi
 
#suffix=$check
#username=$2
 
# С помощью данной команды создастся файл list с построчно разделенными именами
# файлов, имеющих больше 1 связей и с наименованием, оканчивающимся на ".$suffix"
ls -l | grep -v "1 ${username}" | find *.${suffix} > list
 
# С помощью данного цикла переменная filename принимает поочередно
# все имена из полученного ранее файла list
for filename in $(cat list)
do
    # Промежуточная переменная resname отлична от filename
    # лишь тем, что не содержит ".$suffix" на конце.
    # Процедура удаления суффикса выполняется при помощи утилиты sed, заменяя
    # вхождение ".$suffix" в названии файла на "".
    resname=`echo $filename | sed "s/.$suffix//g"`
    # Создание ссылки, синонима файла filename
    # путем переноса в начало resname "$suffix"
    ln "$filename" "./synonyms/${suffix}$resname"
done
