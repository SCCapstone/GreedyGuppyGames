using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEditor;


namespace Tests
{
    public class KillEnemyTest: MonoBehaviour
    {
        private Enemy newEnemy;
        //Spawn location
        private Vector3 pos = new Vector3(-44,5,67);
        private Quaternion rot = new Quaternion(0f,0f,0f, 0f);
        private GameObject grubPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemies/Grub.prefab");

        private GameObject grubObject;

        [SetUp]
        public void LoadScene()
        {
            SceneManager.LoadScene("Level1", LoadSceneMode.Single);
        }
        public void SetUp()
        {
            // Declares grubObject as the grubPrefab
            grubObject = grubPrefab;
        }

        [UnityTest]
        public IEnumerator DieTest()
        {
            // ARRANGE
            Transform grub = grubPrefab.transform;

            // ACT
            try
            {
                //WaveSpawner.SpawnEnemy(grub);
                GameObject killMe = (GameObject)Instantiate(grubPrefab, pos, rot);
                killMe.GetComponent<Enemy>().Die();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                Assert.Fail("Enemy didnt die",grub);
                throw;
            }
            //Figure out how to call Die() on the spawned enemy here, maybe
            yield return new WaitForSeconds(3f);

            // ASSERT
            //if no excpetion, passes else fails
            Assert.Pass("Enemy successfully died",grub);
            //Assert.Fail("SpawnEnemy failed to spawn an enemy",grub);
        }
            
        [TearDown]
        public void TearDown()
        {
            GameObject.Destroy(grubObject);
            //SceneManager.UnloadSceneAsync("MainScene");
        }   
    }
}