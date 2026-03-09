using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

//[Preserve]
//public class DummyReference
//{
    //[Preserve]
    // static DummyReference()
    // {
    //     // 这里列出所有需要保留的类型，防止被裁剪掉
    //     {
    //         var save_1 = typeof(UnityEngine.Collider2D);
    //     }
    //     {
    //         var save_2 = typeof(UnityEngine.Physics2D);}
    //     {
    //         var save_1 = typeof(Sirenix.OdinInspector.SerializedMonoBehaviour);
    //     }
    //     {
    //         var save_1 = typeof(Sirenix.Serialization.OdinSerializeAttribute);
    //     }
    //     {
    //         var save_1 = typeof(System.CodeDom.Compiler.GeneratedCodeAttribute);
    //     }
    //     {
    //         var save_1 = typeof(System.ComponentModel.EditorBrowsableAttribute);
    //     }
    //     {
    //         var save_1 = typeof(System.ComponentModel.EditorBrowsableState);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.InputSystem.Layouts.InputControlAttribute);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.InputSystem.OnScreen.OnScreenControl);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.Camera);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.Color);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.Component);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.Coroutine);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.Debug);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.GameObject);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.Gizmos);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.Mathf);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.MonoBehaviour);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.Object);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.Quaternion);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.RectTransform);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.ScriptableObject);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.Scripting.PreserveAttribute);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.SerializeField);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.Time);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.Transform);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.Vector2);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.Vector3);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.Collider2D);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.Physics2D);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.EventSystems.IDragHandler);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.EventSystems.IEventSystemHandler);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.EventSystems.IPointerDownHandler);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.EventSystems.IPointerUpHandler);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.EventSystems.PointerEventData);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.Canvas);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.RectTransformUtility);
    //     }
    //     {
    //         var save_1 = typeof(UnityEngine.RenderMode);
    //     }
    //     {
    //         var save_1 = typeof(YooAsset.AssetHandle);
    //     }
    //     {
    //         var save_1 = typeof(YooAsset.HandleBase);
    //     }
    //     {
    //         var save_1 = typeof(YooAsset.ResourcePackage);
    //     }
    //     {
    //         var save_1 = typeof(YooAsset.YooAssets);
    //     }
    //     {
    //         var save_1 = typeof(System.Action);
    //     }
    //     {
    //         var save_1 = typeof(System.Array);
    //     }
    //     {
    //         var save_1 = typeof(System.Byte);
    //     }
    //     {
    //         var save_1 = typeof(System.Collections.IEnumerator);
    //     }
    //     {
    //         var save_1 = typeof(System.Delegate);
    //     }
    //     {
    //         var save_1 = typeof(System.Diagnostics.DebuggableAttribute);
    //     }
    //     {
    //         var save_1 = typeof(System.Diagnostics.DebuggableAttribute);
    //     }
    //     {
    //         var save_1 = typeof(System.Diagnostics.DebuggerHiddenAttribute);
    //     }
    //     {
    //         var save_1 = typeof(System.Enum);
    //     }
    //     {
    //         var save_1 = typeof(System.IDisposable);
    //     }
    //     {
    //         var save_1 = typeof(System.NotSupportedException);
    //     }
    //     {
    //         var save_1 = typeof(System.Object);
    //     }
    //     {
    //         var save_1 = typeof(System.Runtime.CompilerServices.CompilationRelaxationsAttribute);
    //     }
    //     {
    //         var save_1 = typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute);
    //     }
    //     {
    //         var save_1 = typeof(System.Runtime.CompilerServices.IteratorStateMachineAttribute);
    //     }
    //     {
    //         var save_1 = typeof(System.Runtime.CompilerServices.RuntimeCompatibilityAttribute);
    //     }
    //     {
    //         var save_1 = typeof(System.Runtime.CompilerServices.RuntimeHelpers);
    //     }
    //     {
    //         var save_1 = typeof(System.RuntimeFieldHandle);
    //     }
    //     {
    //         var save_1 = typeof(System.RuntimeTypeHandle);
    //     }
    //     {
    //         var save_1 = typeof(System.String);
    //     }
    //     {
    //         var save_1 = typeof(System.Type);
    //     }
    //     {
    //         var save_1 = typeof(System.ValueType);
    //     }
    //}
//}
//