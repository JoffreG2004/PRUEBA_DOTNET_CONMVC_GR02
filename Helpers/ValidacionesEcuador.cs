using System.Text.RegularExpressions;

namespace RESTAURANT_CONMVC_DONET_BOLANOS_LUCIANA.Helpers
{
    public static class ValidacionesEcuador
    {
        private static readonly Regex SoloLetrasRegex = new Regex(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ\s]+$", RegexOptions.Compiled);
        private static readonly Regex EmailRegex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$", RegexOptions.Compiled);

        public static bool SoloLetras(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) || SoloLetrasRegex.IsMatch(valor.Trim());
        }

        public static bool EmailValido(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) || EmailRegex.IsMatch(valor.Trim());
        }

        public static bool CelularEcuador(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) || Regex.IsMatch(valor.Trim(), @"^09\d{8}$");
        }

        public static bool CedulaEcuador(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
            {
                return true;
            }

            cedula = cedula.Trim();

            if (cedula == "9999999999")
            {
                return true;
            }

            if (!Regex.IsMatch(cedula, @"^\d{10}$"))
            {
                return false;
            }

            int provincia = int.Parse(cedula.Substring(0, 2));
            int tercerDigito = int.Parse(cedula.Substring(2, 1));

            if (provincia < 1 || provincia > 24 || tercerDigito > 5)
            {
                return false;
            }

            int suma = 0;
            for (int i = 0; i < 9; i++)
            {
                int digito = int.Parse(cedula.Substring(i, 1));
                if (i % 2 == 0)
                {
                    digito *= 2;
                    if (digito > 9)
                    {
                        digito -= 9;
                    }
                }

                suma += digito;
            }

            int verificador = (10 - (suma % 10)) % 10;
            return verificador == int.Parse(cedula.Substring(9, 1));
        }
    }
}
