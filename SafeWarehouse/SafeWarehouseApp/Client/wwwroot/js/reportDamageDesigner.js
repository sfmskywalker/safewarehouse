async function updateDamageSpriteAsync(e) {
    const target = e.target;
    const dataset = target.dataset;
    const damageId = dataset.damageId;
    const rect = target.getBoundingClientRect();
    const parentRect = target.parentElement.getBoundingClientRect();
    const parentScrollLeft = target.parentElement.scrollLeft;
    const parentScrollTop = target.parentElement.scrollTop;
    const left = Math.round(rect.left - parentRect.left + parentScrollLeft);
    const top = Math.round(rect.top - parentRect.top + parentScrollTop);
    const width = Math.round(rect.width);
    const height = Math.round(rect.height);
    await DotNet.invokeMethodAsync('SafeWarehouseApp.Client', 'UpdateDamageSpriteAsyncCaller', damageId, left, top, width, height);
}

export function initialize() {
    interact(".damage").origin('parent').draggable({
        modifiers: [
            interact.modifiers.restrictRect({
                restriction: 'parent'
            })
        ],
        listeners: {
            move: (e) => {
                const target = e.target;
                const x = (parseFloat(target.getAttribute("data-x")) || 0) + e.dx;
                const y = (parseFloat(target.getAttribute("data-y")) || 0) + e.dy;

                target.style.transform = `translate(${x}px, ${y}px)`;
                target

                target.setAttribute("data-x", x);
                target.setAttribute("data-y", y);
            },
            end: async (e) => {
                await updateDamageSpriteAsync(e);
            }
        }
    }).resizable({
        edges: {top: true, left: true, bottom: true, right: true},
        invert: 'reposition',
        modifiers: [
            interact.modifiers.aspectRatio({
                ratio: 1,
                modifiers: [
                    interact.modifiers.restrictSize({max: 'parent'})
                ]
            }),
            interact.modifiers.restrictRect({
                restriction: 'parent'
            })
        ],
        listeners: {
            move: function (event) {
                let {x, y} = event.target.dataset

                x = (parseFloat(x) || 0) + event.deltaRect.left
                y = (parseFloat(y) || 0) + event.deltaRect.top

                Object.assign(event.target.style, {
                    width: `${event.rect.width}px`,
                    height: `${event.rect.height}px`,
                    transform: `translate(${x}px, ${y}px)`
                })

                Object.assign(event.target.dataset, {x, y})
            },
            end: async (e) => {
                await updateDamageSpriteAsync(e);
            }
        }
    });
}