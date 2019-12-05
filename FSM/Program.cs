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
                          (0, '0', 'Z') => (0, "0"),
                          (0, '0', '0') => (0, "00"),
                          (0, '1', '0') => (1, "1"),
                          (1, '1', '1') => (1, "e"),
                          (1, '1', '0') => (1, "1"),
                          (1, '0', 'Z') => (2, "0"),
                          (2, '0', '0') => (1, "e"),
                          (1, 'e', 'Z') => (2, "e")
                      };
            //Объект,являющийся автоматом 
            var fsm = new FSM()
            {
                States = new int[] { 0, 1, 2 },
                FiniteStates = new int[] { 2 },
                Map = map
            };
            //проверяемая стока
            var start_input = "000011111111000000";
            //число нулей до единиц
            int n = 0;
            //число нулей после последовательности единиц
            int m = 0;
            //индекс
            int i = 0;
            var stack_size = 0;
            while (start_input[i] != '1' && i<start_input.Length)
            {
                n++;
                i++;
            }
            for (int j = n; j < start_input.Length; j++)
            {
                if (start_input[j] == '0')
                    m++;
            }
            if (n > m)
                stack_size = n;
            else stack_size = m;
            //строка,с символом пустой строки в конце для упрощения работы 
            var input = start_input + "e";
            
            bool result = Result(fsm,input,stack_size);
            Console.WriteLine(result);
        }
        //функция,реализующая работу автомата
        static bool Result(FSM fsm,string input,int stack_size)
        {
            //результат
            var result = false;
            //Стек
            Stack<char> stack = new Stack<char>(stack_size);
            //Сразу кладем в него "Z" (показатель дна)
            stack.Push('Z');
            //Проверяем цепочку
            for (int i = 0; i < input.Length; i++)
            {
                var state = 0;
                var canExecuteFSM = true;
                var index = i;


                do
                {
                    //Проверяем,принадлежит ли текущее состояние множеству состояний нашего автомата
                    if (fsm.States.ToList().Contains(state))
                    {

                            if (index < input.Length)
                            {
                                //вызываем функцию переходов
                                var temp = fsm.Map(state, input[index], stack.Peek());
                                state = temp.Item1;

                            //Проверяем ,является ли записываемый в стек символ символов пустой строки
                            if (temp.Item2[0] != 'e')
                            {
                                //Стираю j-1 символ,где j-число элементов,которые надо записать в стек
                                for(int k=0;k<temp.Item2.Length-1;k++)
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

                } while (canExecuteFSM);
                //Проеряем ,является наш стек пустым после проверки цепочки
                if (state==fsm.FiniteStates[0] && stack.Count==0)
                    result = true;
            }
            return result;
        }
    }
 
}
