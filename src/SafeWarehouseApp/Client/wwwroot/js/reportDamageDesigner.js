export function initialize() {
    console.debug("Initializing designer...");
    initInteract();
    initHammer();
    console.debug("Designer initialized.");
}

function initInteract() {

    async function updateDamageSpriteAsync(e) {
        const target = e.target;
        const dataset = target.dataset;
        const locationId = dataset.locationId;
        const rect = target.getBoundingClientRect();
        const parentRect = target.parentElement.getBoundingClientRect();
        const parentScrollLeft = target.parentElement.scrollLeft;
        const parentScrollTop = target.parentElement.scrollTop;
        const left = Math.round(rect.left - parentRect.left + parentScrollLeft);
        const top = Math.round(rect.top - parentRect.top + parentScrollTop);
        const width = Math.round(rect.width);
        const height = Math.round(rect.height);
        target.style.transform = 'none';
        target.style.left = `${left}px`;
        target.style.top = `${top}px`;
        target.setAttribute('data-x', 0);
        target.setAttribute('data-y', 0);
        await DotNet.invokeMethodAsync('SafeWarehouseApp.Client', 'UpdateDamageSpriteAsyncCaller', locationId, left, top, width, height);
    }

    interact(".damage").origin('parent').draggable({
        modifiers: [
            interact.modifiers.restrictRect({
                restriction: 'parent'
            })
        ],
        listeners: {
            move: (e) => {
                const target = e.target;
                let x = (parseFloat(target.getAttribute("data-x")) || 0) + e.dx;
                let y = (parseFloat(target.getAttribute("data-y")) || 0) + e.dy;

                if (x > 800)
                    x = 800;

                if (y > 800)
                    y = 800;

                target.style.transform = `translate(${x}px, ${y}px)`;

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

function initHammer() {
    const designerCanvasElement = document.querySelector('.design-canvas');
    const hammer = new Hammer(designerCanvasElement, {domEvents: true});

    hammer.on('doubletap', async function (ev) {
        const left = ev.srcEvent.layerX;
        const top = ev.srcEvent.layerY;
        const locationElement = ev.target.closest('.damage');

        if (locationElement)
            await DotNet.invokeMethodAsync('SafeWarehouseApp.Client', 'DoubleTapLocationAsyncCaller', locationElement.dataset['locationId']);
        else
            await DotNet.invokeMethodAsync('SafeWarehouseApp.Client', 'DoubleTapDesignerAsyncCaller', left, top);
    });

    hammer.on('tap', async function (ev) {
        const locationElement = ev.target.closest('.damage');

        if (locationElement)
            await DotNet.invokeMethodAsync('SafeWarehouseApp.Client', 'DoubleTapLocationAsyncCaller', locationElement.dataset['locationId']);
    });
}