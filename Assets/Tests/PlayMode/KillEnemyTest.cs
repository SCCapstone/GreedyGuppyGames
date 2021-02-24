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
        private GameObject newEnemy;
        //Spawn location
        private Vector3 pos = new Vector3(-44,5,67);
        private Quaternion rot = new Quaternion(0f,0f,0f, 0f);
        private GameObject grubPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemies/Grub.prefab");
        [SetUp]
        public void LoadScene()
        {
            SceneManager.LoadScene("Level1", LoadSceneMode.Single);
        }

        [UnityTest]
        public IEnumerator DieTest()
        {
            // ARRANGE
            newEnemy = (GameObject)Instantiate(grubPrefab, pos, rot);

            // ACT
            try
            {
                //WaveSpawner.SpawnEnemy(grub);
                
                newEnemy.GetComponent<Enemy>().Die();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                Assert.Fail("Enemy didnt die",newEnemy);
                throw;
            }
            //Figure out how to call Die() on the spawned enemy here, maybe
            yield return null;

            // ASSERT
            //if no excpetion, passes else fails
            
            Assert.Pass("Enemy successfully died",newEnemy);
            //Assert.Fail("SpawnEnemy failed to spawn an enemy",grub);
        }
            
        [TearDown]
        public void TearDown()
        {
            if(newEnemy != null)
            {
                Destroy(newEnemy);
            }
        }   
    }
}