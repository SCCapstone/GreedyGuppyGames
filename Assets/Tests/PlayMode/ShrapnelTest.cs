using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEditor;


namespace Tests
{
    public class ShrapnelTest: MonoBehaviour
    {
        private GameObject[] bullets;
        private GameObject bullet;
        //Spawn location
        private Vector3 pos = new Vector3(-44,5,67);
        private Quaternion rot = new Quaternion(0f,0f,0f, 0f);
        private GameObject bulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ammo/CannonShotLargeShrapnel.prefab");
        [SetUp]
        public void LoadScene()
        {
            SceneManager.LoadScene("Level1", LoadSceneMode.Single);
        }

        [UnityTest]
        public IEnumerator MakeShrapnelTest()
        {
            bullet = (GameObject)Instantiate(bulletPrefab, pos, rot);
            try
            {
                bullet.GetComponent<Bullet>().MakeShrapnel();
                bullets = GameObject.FindGameObjectsWithTag("Ammo");
                Assert.AreEqual(9,bullets.Length);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                Assert.Fail("Did not make shrapnel",bullet);
                throw;
            }
            yield return null;
            
            Assert.Pass("Made shrapnel",bullet);
        }
            
        [TearDown]
        public void TearDown()
        {
            Destroy(bullet);
            foreach (GameObject go in bullets)
            {
                Destroy(go);
            }
        }   
    }
}