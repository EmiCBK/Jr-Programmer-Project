using UnityEngine;

// Hereda el movimiento y la selección de Unit.
public class ProductivityUnit : Unit
{
    public float ProductivityMultiplier = 2f;

    private ResourcePile m_CurrentPile;

    protected override void BuildingInRange()
    {
        // Evita multiplicar la producción en cada frame.
        if (m_CurrentPile != null)
            return;

        ResourcePile pile = m_Target as ResourcePile;

        if (pile != null)
        {
            m_CurrentPile = pile;
            m_CurrentPile.ProductionSpeed *= ProductivityMultiplier;
        }
    }

    private void ResetProductivity()
    {
        if (m_CurrentPile != null)
        {
            m_CurrentPile.ProductionSpeed /= ProductivityMultiplier;
            m_CurrentPile = null;
        }
    }

    public override void GoTo(Building target)
    {
        ResetProductivity();
        base.GoTo(target);
    }

    public override void GoTo(Vector3 position)
    {
        ResetProductivity();
        base.GoTo(position);
    }
}