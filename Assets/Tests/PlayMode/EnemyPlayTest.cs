using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEditor;

namespace Tests
{
    public class EnemyPlayTest
    {
        private Enemy newEnemy;
        //Spawn location
        private Vector3 pos = new Vector3(-44,5,67);
        private GameObject grubPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemies/Grub.prefab");

        private GameObject grubObject;

        [SetUp]
        public void LoadScene()
        {
            SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        }
        public void SetUp()
        {
            // Declares grubObject as the grubPrefab
            grubObject = grubPrefab;
            //GameObject grubObject = GameObject.Find("TestGrub");
            // Debug.Log(grubObject); // This object is never found
            //var prefabInstance = Object.Instantiate(grubObject,pos,Quaternion.identity);
            //newEnemy = GameObject.Instantiate(grubObject,pos,Quaternion.identity).GetComponent<Enemy>();
            //newEnemy = GameObject.Instantiate(grubPrefab,pos,Quaternion.identity).GetComponent<Enemy>();
            
            // testObject = GameObject.Instantiate(new GameObject());
            // enemy = testObject.AddComponent<Enemy>();
        }

        // Test not functional
        // [UnityTest]
        // public IEnumerator EnemyDeathTest()
        // {
        //     newEnemy = grubObject.AddComponent<Enemy>();

        //     GameObject instantiate = GameObject.Instantiate(grubPrefab,pos,Quaternion.identity);
        //     instantiate.gameObject.AddComponent<Enemy>();

        //     // NEED TO SPAWN AN ENEMY TO CALL THE DIE function


        //     newEnemy = GameObject.Instantiate(grubObject,pos,Quaternion.identity).GetComponent<Enemy>();
        //     newEnemy.Die();
        //     yield return new WaitForSeconds(3f);
        // }

        // Working, now need to assert that it actually happens
        [UnityTest]
        public IEnumerator EnemySpawnTest()
        {
            // ARRANGE
            Transform grub = grubPrefab.transform;

            // ACT
            try
            {
                WaveSpawner.SpawnEnemy(grub);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                Assert.Fail("SpawnEnemy failed to spawn an enemy",grub);
                throw;
            }
            //Figure out how to call Die() on the spawned enemy here, maybe
            yield return new WaitForSeconds(3f);

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