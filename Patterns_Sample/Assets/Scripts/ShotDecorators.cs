public interface IShotDecorator
{
    void Shoot(ShootCommand shootCommand);
}

public class NormalShotDecorator : IShotDecorator
{
    public void Shoot(ShootCommand shootCommand)
    {
        shootCommand.ShootOneBullet();
    }
}

public class TripleShotDecorator : IShotDecorator
{
    public void Shoot(ShootCommand shootCommand)
    {
        shootCommand.StartCoroutine(shootCommand.ShootThreeBullets());
    }
}
