// <copyright file="OptionsMenu.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using Core.UI;
using TowerDefense.Game;
using UnityEngine.UI;

namespace TowerDefense.UI
{
    /// <summary>
    /// Simple options menu for setting volumes
    /// </summary>
    public class OptionsMenu : SimpleMainMenuPage
    {
        public Slider masterSlider;

        public Slider sfxSlider;

        public Slider musicSlider;

        /// <summary>
        /// Event fired when sliders change
        /// </summary>
        public void UpdateVolumes()
        {
            float masterVolume, sfxVolume, musicVolume;
            this.GetSliderVolumes(out masterVolume, out sfxVolume, out musicVolume);

            if (GameManager.instanceExists)
            {
                GameManager.instance.SetVolumes(masterVolume, sfxVolume, musicVolume, false);
            }
        }

        /// <summary>
        /// Set initial slider values
        /// </summary>
        public override void Show()
        {
            if (GameManager.instanceExists)
            {
                float master, sfx, music;
                GameManager.instance.GetVolumes(out master, out sfx, out music);

                if (this.masterSlider != null)
                {
                    this.masterSlider.value = master;
                }

                if (this.sfxSlider != null)
                {
                    this.sfxSlider.value = sfx;
                }

                if (this.musicSlider != null)
                {
                    this.musicSlider.value = music;
                }
            }

            base.Show();
        }

        /// <summary>
        /// Persist volumes to data store
        /// </summary>
        public override void Hide()
        {
            float masterVolume, sfxVolume, musicVolume;
            this.GetSliderVolumes(out masterVolume, out sfxVolume, out musicVolume);

            if (GameManager.instanceExists)
            {
                GameManager.instance.SetVolumes(masterVolume, sfxVolume, musicVolume, true);
            }

            base.Hide();
        }

        /// <summary>
        /// Retrieve values from sliders
        /// </summary>
        private void GetSliderVolumes(out float masterVolume, out float sfxVolume, out float musicVolume)
        {
            masterVolume = this.masterSlider != null ? this.masterSlider.value : 1;
            sfxVolume = this.sfxSlider != null ? this.sfxSlider.value : 1;
            musicVolume = this.musicSlider != null ? this.musicSlider.value : 1;
        }
    }
}