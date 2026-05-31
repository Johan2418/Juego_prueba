# Checklist de revision para Unity

Antes de aceptar cambios de Codex, revisar:

## Codigo

- [ ] El script tiene una responsabilidad clara.
- [ ] No hay logica innecesariamente compleja.
- [ ] No se usa `FindObjectOfType` sin motivo.
- [ ] No se usa `GameObject.Find` repetidamente.
- [ ] Las variables publicas son necesarias.
- [ ] Los campos de Inspector usan `[SerializeField]` cuando corresponde.
- [ ] No hay dependencias ocultas dificiles de configurar.
- [ ] El codigo tiene nombres claros.

## Unity

- [ ] Se indico donde colocar el script.
- [ ] Se explicaron los componentes necesarios.
- [ ] Se indicaron referencias del Inspector.
- [ ] No se rompieron prefabs ni escenas.
- [ ] Se mantuvieron archivos `.meta`.

## Gameplay

- [ ] La mecanica se puede probar facilmente.
- [ ] El comportamiento esperado esta explicado.
- [ ] No se agrego complejidad fuera del alcance.
- [ ] La demo sigue siendo pequena y manejable.

## Documentacion

- [ ] README actualizado si cambia la estructura.
- [ ] GAME_DESIGN actualizado si cambia una mecanica.
- [ ] ROADMAP actualizado si se completa una fase.
