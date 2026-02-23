using Microsoft.AspNetCore.Mvc;

namespace Calculator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculatorController : ControllerBase
    {


        private readonly ILogger<CalculatorController> _logger;

        public CalculatorController(ILogger<CalculatorController> logger)
        {
            _logger = logger;
        }

        [HttpGet("multiply/{firstNumber}/{secundNumber}")]
        public IActionResult GetMultiply(string firstNumber, string secundNumber)
        {
            if (ValidationNumber(firstNumber) && ValidationNumber(secundNumber))
            {
                decimal result = ConvertDecimal(firstNumber) * ConvertDecimal(secundNumber);
                return Ok(result);
            }
            return BadRequest("Invalid input.");
        }

        [HttpGet("subtray/{firstNumber}/{secundNumber}")]
        public IActionResult GetSubtray(string firstNumber, string secundNumber)
        {
            if (ValidationNumber(firstNumber) && ValidationNumber(secundNumber))
            {
                decimal result = ConvertDecimal(firstNumber) - ConvertDecimal(secundNumber);
                return Ok(result);
            }
            return BadRequest("Invalid input.");
        }

        [HttpGet("diviply/{firstNumber}/{secundNumber}")]
        public IActionResult GetDiviplay(string firstNumber, string secundNumber)
        {
            ObjectResult resultRequest = BadRequest("Invalid input.");
            
            if (ValidationNumber(firstNumber) && ValidationNumber(secundNumber))
            {
                if(ConvertDecimal(secundNumber) == 0)
                {
                    resultRequest = BadRequest("Division by zero is not allowed.");
                }
                decimal result = ConvertDecimal(firstNumber) / ConvertDecimal(secundNumber);
                resultRequest = Ok(result);
            }
            return resultRequest;
        }

        [HttpGet("med/{firstNumber}/{secundNumber}")]
        public IActionResult GetMed(string firstNumber, string secundNumber)
        {
            ObjectResult resultRequest = BadRequest("Invalid input.");

            if (ValidationNumber(firstNumber) && ValidationNumber(secundNumber))
            { 
              
                decimal result = (ConvertDecimal(firstNumber) + ConvertDecimal(secundNumber))/2;
                resultRequest = Ok(result);
            }
            return resultRequest;
        }

        [HttpGet("squad/{number}")]
        public IActionResult GetSquad(string number)
        {
            ObjectResult resultRequest = BadRequest("Invalid input.");

            if (ValidationNumber(number))
            {
                double result = Math.Sqrt( ConvertDouble(number)) ;
                resultRequest = Ok(result);
            }
            return resultRequest;
        }

        private double ConvertDouble(string strNumber)
        {
            double valueNumber = 0;
            var cultureBrazil = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
            double.TryParse(strNumber, System.Globalization.NumberStyles.Any, cultureBrazil, out valueNumber);
            return valueNumber;
        }

        [HttpGet("sum/{firstNumber}/{secundNumber}")]
        public IActionResult GetSum(string firstNumber, string secundNumber)
        {
            if (ValidationNumber(firstNumber) && ValidationNumber(secundNumber))
            {
                decimal sum = ConvertDecimal(firstNumber) + ConvertDecimal(secundNumber);
                return Ok(sum);
            }
            return BadRequest("Invalid input.");
        }

        private decimal ConvertDecimal(string strNumber)
        {
            decimal valueNumber = 0;
            var cultureBrazil = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
            decimal.TryParse(strNumber, System.Globalization.NumberStyles.Any, cultureBrazil, out valueNumber);
            return valueNumber;

        }

        private bool ValidationNumber(string strNumber)
        {
            if (string.IsNullOrEmpty(strNumber) ||
                (strNumber.Contains(".") && strNumber.Contains(",") &&
                 strNumber.IndexOf(".") < strNumber.IndexOf(",")) ||
                 (strNumber.Contains(".") && !strNumber.Contains(",")))
            {
                return false;
            }
            var cultureBrazil = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
            return Decimal.TryParse(strNumber, System.Globalization.NumberStyles.Any, cultureBrazil, out _);
        }
    }
}
