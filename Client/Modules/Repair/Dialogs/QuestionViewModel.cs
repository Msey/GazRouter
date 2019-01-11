using System;
using System.Collections.Generic;
using GazRouter.Common.ViewModel;

namespace GazRouter.Repair.Dialogs
{
    public class QuestionViewModel : DialogViewModel<Action<int>>
    {
        public QuestionViewModel(Action<int> callback)
            : base(callback) { }

        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Вопрос 
        /// </summary>
        public string Question { get; set; }


        /// <summary>
        /// Список ответов
        /// </summary>
        public List<Answer> AnswerList { get; set; }

        private Answer _selectedAnswer;
        public Answer SelectedAnswer
        {
            get { return _selectedAnswer; }
            set
            {
                _selectedAnswer = value;

                if (value != null)
                {
                    DialogResult = true;
                }

            }
        }

        protected override void InvokeCallback(Action<int> closeCallback)
        {
            closeCallback(SelectedAnswer.Num);
        }
    }


    public class Answer
    {
        public int Num { get; set; }
        public string Text { get; set; }
    }


}