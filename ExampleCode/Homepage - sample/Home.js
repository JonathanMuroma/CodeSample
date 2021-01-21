import React, {useState} from 'react'
import ProjectSample from './ProjectSample.js'
import projectData from './projectData.js'
import scott_pilgrim_pixel_art from '../../images/scott_pilgrim_pixel_art.png'
import './Home.css'

function Home() {
    
    const projectComponents = projectData.map(item => <ProjectSample key={item.id} item={item}/>)
    // const [data, setData] = useState(projectData);
    console.log(projectData[0].title)
    return(
        <div className="Home">
            <div className="IntroductionBackground">
                <div className="IntroductionBackgroundGradient">
                    <h2>Introduction</h2>
                    <div className="Introduction">
                        <img src={scott_pilgrim_pixel_art} alt="Sprite of me" />
                        <p>
                            My name is Joonatan Muroma and welcome to Cerealland, the home of my works!
                        </p>
                        <p>
                            Here you can find information regarding me and my skills, 
                            the projects that I've worked on and other miscellaneous things I've done. 
                        </p>
                        <p>
                            I'm currently a student at Metropolia university, where I study information technology, 
                            and I'm nearing the end of my studies. 
                            I am currently looking for a job in the software industry and I have created this site 
                            to showcase my capabilities as a programmer and the various projects 
                            that I have been part of.
                        </p>
                    </div>
                </div>
            </div>
            
            {/* <ProjectSample title="Guns of Wreckage"/> */}
            {projectComponents}
        </div>
    );
}

export default Home;