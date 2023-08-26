using Gameplay.Interaction;
using UnityEngine;

namespace Gameplay.Dice
{
    public class MagmaDice : Dice
    {
        [SerializeField] private ParticleSystem sparkParticles;

        private void OnCollisionEnter(Collision collision)
        {
            ContactPoint[] contacts = new ContactPoint[10];
            int contactsLength = collision.GetContacts(contacts);
            Vector3 medianPoint = Vector3.zero;
            for (int i = 0; i < contactsLength; i++) medianPoint += contacts[i].point;
            medianPoint /= contactsLength;
            sparkParticles.transform.position = medianPoint;
            sparkParticles.Play();
        }

        public override void OnClick(InteractionResult result)
        {
            base.OnClick(result);
            sparkParticles.transform.position = result.IntersectionPoint;
            sparkParticles.Play();
        }
    }
}