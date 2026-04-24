namespace ContaCorrente.Domain.ValueObjects;

public class Cpf
{
    public string Numero { get; private set; }

    private Cpf() { }

    public Cpf(string numero)
    {
        if (!EhValido(numero))
            throw new ArgumentException("CPF inválido");

        Numero = ApenasNumeros(numero);
    }

    public override string ToString()
        => Numero;

    // Validação
    private static bool EhValido(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        cpf = ApenasNumeros(cpf);

        if (cpf.Length != 11)
            return false;

        // Elimina CPFs inválidos conhecidos
        if (new string(cpf[0], 11) == cpf)
            return false;

        int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        var tempCpf = cpf.Substring(0, 9);
        var soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        var resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        var digito = resto.ToString();
        tempCpf += digito;

        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        digito += resto.ToString();

        return cpf.EndsWith(digito);
    }

    private static string ApenasNumeros(string input)
    {
        return new string(input.Where(char.IsDigit).ToArray());
    }
}