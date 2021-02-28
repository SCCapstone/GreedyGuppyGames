using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEditor;


namespace Tests
{
    public class EnemyPlayTest
    {
        //Spawn location
        private Vector3 pos = new Vector3(-44,5,67);
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
        // Working, now need to assert that it actually happens
        [UnityTest]
        public IEnumerator EnemySpawnTest()
        {
            // ARRANGE
            Transform grub = grubPrefab.transform;

            // ACT
            try
            {
                //GameObject.FindGameObjectWithTag("wavespawner").GetComponent<WaveSpawner>().SpawnEnemy(grub);
                GameObject.Find("GameMaster").GetComponent<WaveSpawner>().SpawnEnemy(grub);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                Assert.Fail("SpawnEnemy failed to spawn an enemy",grub);
                throw;
            }
            //Figure out how to call Die() on the spawned enemy here, maybe
            yield return null;
            //yield return new WaitForSeconds(3);

            // ASSERT
            //if no excpetion, passes else fails
            Assert.Pass("SpawnEnemy successfully spawned an enemy",grub);
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