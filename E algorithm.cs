using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class script : MonoBehaviour {
    List<critter> critters  = new List<critter>();
    int dontUpdate = 0;
    public float thrust = 1;
    float timeLeft =  10.0f;
    int numCrits = 280;
    int genNum= 0;
    int stopped = 0;
	// Use this for initialization
	void Start () {
		for (int i = 0; i< numCrits; i++){
			GameObject indv= new GameObject();


			GameObject cube= GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.AddComponent<coll>();
			cube.transform.position = new Vector3(0f, 2f , -1000f + i*7f + 0.7f);
			cube.transform.localScale= new Vector3(1f, 2f, 3f );
			Rigidbody cuberig = cube.AddComponent<Rigidbody>();

			GameObject cap1 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
			cap1.transform.position = new Vector3(0, 0, -1000 + i*7);
			CharacterJoint cap1joint = cap1.AddComponent<CharacterJoint>();
			cap1joint.connectedBody = cuberig;

			GameObject cap2 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        	cap2.transform.position = new Vector3(0, 0, -1000f + i*7 + 1.4f);
        	CharacterJoint cap2joint = cap2.AddComponent<CharacterJoint>();
			cap2joint.connectedBody = cuberig;

			cube.transform.parent = indv.transform;

			cap1.transform.parent = indv.transform;
			cap2.transform.parent = indv.transform;

			critter crit = new critter();
			crit.gameObj = indv;
			
			
			for (int j =0; j< Random.Range(1,20); j++){
				action act = new action();
				act.setAct(Random.Range(0,2));
				act.limb = Random.Range(1,3);
				act.val = new Vector3 ( Random.Range(0,50),0, 0) ;
				act.num = 1;
				crit.actionseq.Add(act);
			
			}
		critters.Add(crit);

        }
	}
	
	// Update is called once per frame
	void Update () {

		if (dontUpdate == 0)
{

		for (int i =0; i< numCrits; i++){
			critter crit = critters[i];
			crit.lastPosition = crit.gameObj.transform.position;
			if (crit.curAct  == crit.actionseq.Count-1) {
				crit.curAct = 0;
			}
			if (crit.curAct < crit.actionseq.Count){
				action act1 = crit.actionseq[crit.curAct++];
				//crit.fitness += Vector3.Distance(crit.gameObj.transform.position, crit.lastPosition);

				if (act1.act == "AddForce" ){
						Rigidbody cJ = crit.gameObj.transform.GetChild(act1.limb).gameObject.GetComponent( typeof(Rigidbody) ) as Rigidbody;
						float fudge;
						if (act1.limb == 2 ){
							fudge = 1.4f;
						}
						else{
							fudge = 0f;
						}
						Vector3 pos = new Vector3(0,0, -1000 + i*7+fudge);
						Vector3 force = new Vector3(act1.val.x, act1.val.y, act1.val.z);
						cJ.AddForceAtPosition(force,pos);
					}

				}
		}
		


	     timeLeft -= Time.deltaTime;

     	if (timeLeft <= 0){
     		nextGen();
     	}
	}	
}

	void nextGen(){
		dontUpdate = 1;
		Debug.Log("Gen: " + genNum++);
		for (int i =0; i< numCrits; i++){
			critters[i].fitness = -1 * critters[i].gameObj.transform.GetChild(1).position.x;
			Destroy(critters[i].gameObj);
		}


		List<critter> ordered = critters.OrderBy(o=>o.fitness).ToList();
		Debug.Log("Max reached: " + ordered[0].fitness);
		for (int i =0; i< numCrits; i++){
			int k = 0;
			int m = 0;
			while (k == m){
				k = Random.Range(0,5);
				m = Random.Range(0,5);
			}

			GameObject indv = new GameObject();

			GameObject cube= GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = new Vector3(0f, 2f , -1000f + i*7f + 0.7f);
			cube.transform.localScale= new Vector3(1f, 2f, 3f );

			Rigidbody cuberig = cube.AddComponent<Rigidbody>();
			cube.AddComponent<coll>();

			GameObject cap1 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
			cap1.transform.position = new Vector3(0, 0, -1000 + i*7);
			CharacterJoint cap1joint = cap1.AddComponent<CharacterJoint>();
			cap1joint.connectedBody = cuberig;

			GameObject cap2 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        	cap2.transform.position = new Vector3(0, 0, -1000f + i*7 + 1.4f);
        	CharacterJoint cap2joint = cap2.AddComponent<CharacterJoint>();
			cap2joint.connectedBody = cuberig;

			cube.transform.parent = indv.transform;

			cap1.transform.parent = indv.transform;
			cap2.transform.parent = indv.transform;

			critter crit = new critter();
			crit.gameObj = indv;

			for (int j =0; j < ordered[k].actionseq.Count;j++){
				int which = Random.Range(0,2);
				if (which == 0 || j >= ordered[m].actionseq.Count){
					crit.actionseq.Add(ordered[k].actionseq[j]);
				}
				else{
					crit.actionseq.Add(ordered[m].actionseq[j]);
				}
			}

			int mutate = Random.Range(0,100);
			if (mutate < 20){
			for (int z =0; z< Random.Range(0,5); z++){
				
					action act = new action();
					act.setAct(Random.Range(0,1));
					act.limb = Random.Range(1,3);
					act.val = new Vector3 ( Random.Range(0,5), 0,0) ;
					crit.actionseq[Random.Range(0,crit.actionseq.Count)] = act;
				
			}
		}	
			critters[i] = crit;

		}
		dontUpdate = 0;
		timeLeft =5;
	}
	
}

public class action{
	public string  act;
	public Vector3 val = new Vector3();
	public int limb;
	public int num;
   
	public void setAct(int num){
		if (num == 0){
			act = "AddForce";
		}
		else if (num == 1){
			act = "Wait";
		}
	}



}

public class critter
{
	public GameObject gameObj = new GameObject();
	public List<action> actionseq = new List<action>();
	public int curAct = 0;
	public Vector3 lastPosition = new Vector3();
	public float fitness =0.0f;
	public float timer;


}
