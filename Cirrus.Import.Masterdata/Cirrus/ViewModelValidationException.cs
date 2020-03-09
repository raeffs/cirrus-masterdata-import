using System;

namespace Cirrus.Import.Masterdata.Cirrus
{
    class ViewModelValidationException : Exception
    {
        public ViewModelValidationException()
            : base("The view model is invalid")
        {
        }
    }
}
