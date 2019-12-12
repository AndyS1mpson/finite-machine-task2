using System;
using System.Collections.Generic;
using System.Linq;


namespace FSM
{   //Класс автомата
    public class FSM
    {
        //Множество состояний
        public int[] States { get; set; }
        //Множество конечных состояний
        public int[] FiniteStates { get; set; }
        //функция переходов
        public Func<int,char,char,(int,string)> Map { get; set; }
    }
    class Program
    {

        static void Main(string[] args)
        {
            //Функция переходов  δ(q0,0,z)=>(q1,0)
            Func<int, char, char, (int, string)> map = (state, input, topStack) => (state, input, topStack) switch
                      {
                          (0,'0','Z')=>(0,"0Z"),
                          (0,'0','0')=>(0,"00"),
                          (0,'(','0')=>(0,"0"),
                          (0,'(','Z')=>(1,"(Z"),
                          (0,'1','0')=>(2,"1"),
                          (2,'1','1')=>(2,"e"),
                          (2,'1','0')=>(2,"1"),
                          (1,')','(')=>(3,"e"),
                          (2,')','Z')=>(3,"Z"),
                          (3,'0','Z')=>(4,"0Z"),
                          (4,'0','0')=>(3,"e"),
                          (3,'e','Z')=>(3,"e"),
                           _  => (-1, "ignore")
                      };
            //Объект,являющийся автоматом 
            var fsm = new FSM()
            {
                States = new int[] { 0, 1, 2, 3, 4},
                FiniteStates = new int[] { 3 },
                Map = map
            };
            //проверяемая стока
            //var start_input = "0000(11111111)000000";
            string start_input = Console.ReadLine();
            //строка,с символом пустой строки в конце для упрощения работы 
            string input = start_input + "e";
            //Создаю переменную,в которую запишу результат работы автомата
            var temp = Result(fsm, input);
            bool result = temp.Item1;
            int mistakeNumber = temp.Item2;
            if (result == true)
                Console.WriteLine("Автомат допускает данную цепочку");
            else
            {
                Console.WriteLine("Автомат не допускает данную цепочку");
                Console.WriteLine("С {0} номера пошло несоответствие", mistakeNumber);
            }
        }
        //функция,реализующая работу автомата
        static (bool,int) Result(FSM fsm,string input)
        {
            //результат
            var result = false;
            //Стек
            Stack<char> stack = new Stack<char>();
            //номер символа строки ,с которого пошло нессответствие строки автомату
            int mistakeNumber = 0;
            //Сразу кладем в него "Z" (показатель дна)
            stack.Push('Z');

            var canExecuteFSM = true;
            //Проверяем цепочку
            do
            {
                int finish_state=0;
                int state = 0;
                for (int i = 0; i < input.Length; ++i)
                {
                    var index = i;



                    //Проверяем,принадлежит ли текущее состояние множеству состояний нашего автомата
                    if (fsm.States.ToList().Contains(state))
                    {

                        if (index < input.Length)
                        {
                            //вызываем функцию переходов
                            var temp = fsm.Map(state, input[index], stack.Peek());
                            state = temp.Item1;
                            //проверяем,равно ли текущее состояние состоянию,не входящему в множество состояний
                            //(фактически проверяем на несоответствие строки нашему автомату)
                            if (state == -1)
                            {
                                mistakeNumber = index + 1;
                                canExecuteFSM = false;
                            }
                            else
                            //Проверяем ,является ли записываемый в стек символ символом пустой строки
                            if (temp.Item2[0] != 'e')
                            {
                                //Стираю j-1 символ,где j-число элементов,которые надо записать в стек
                                if (temp.Item2.Length == 1)
                                    stack.Pop();
                                else
                                for (int k = 0; k < temp.Item2.Length-1; k++)
                                    stack.Pop();
                                //Записываю j символов
                                for (int j = temp.Item2.Length - 1; j >= 0; j--)
                                    stack.Push(temp.Item2[j]);
                            }
                            else stack.Pop();

                        }
                        else
                        {
                            canExecuteFSM = false;
                        }
                    }
                    else
                    {
                        canExecuteFSM = false;
                    }
                    finish_state = state;

                }
                //Проеряем ,является наш стек пустым после проверки цепочки
                if (finish_state == fsm.FiniteStates[0] && stack.Count == 0)
                { 
                    result = true;
                    canExecuteFSM = false;
                }
                
            } while (canExecuteFSM);
            return (result,mistakeNumber);
        }
    }
 
}
